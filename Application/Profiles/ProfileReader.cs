using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly AppDbContext _context;
        private readonly IUserAccessor _userAccessor;
        public ProfileReader(AppDbContext context, IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
            _context = context;
        }
        public async Task<AttendeeProfile> ReadProfile(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);

            if (user == null) return null;
            // throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

            var currentUser = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

            var profile = new AttendeeProfile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Photos = user.Photos,
                Bio = user.Bio,
                FollowersCount = user.Followers.Count(),
                FollowingsCount = user.Followings.Count(),
            };

            if (currentUser.Followings.Any(x => x.TargetId == user.Id))
            {
                profile.Following = true;
            }

            return profile;
        }
    }
}