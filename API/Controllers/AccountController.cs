using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")] // endpoint need to go with {} 
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _userManager.Users.Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                return CreateUser(user);
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDTO.Email))
            {
                // handle bad request this way is not efficient, and additional error may occured
                // return BadRequest("Email is already taken.");

                // need to handle bad request like this way!
                ModelState.AddModelError("email", "Email already taken.");
                return ValidationProblem(ModelState);
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDTO.UserName))
            {
                ModelState.AddModelError("username", "Username already taken.");
                return ValidationProblem(ModelState);
            }

            var user = new ApplicationUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.UserName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                return CreateUser(user);
            }

            return BadRequest("failed while registering user");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            // get current user using ClaimsType.Email,
            // also display the main photo of user 
            var user = await _userManager.Users.Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            return CreateUser(user);
        }

        private UserDTO CreateUser(ApplicationUser user)
        {
            return new UserDTO
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url, // get main photo
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}
