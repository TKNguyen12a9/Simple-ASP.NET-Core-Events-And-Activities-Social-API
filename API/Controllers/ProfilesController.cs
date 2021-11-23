using System.Threading.Tasks;
using Application.Activities;
using Application.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ProfilesController : BaseApiController
	{
		[HttpGet("{username}")]
		public async Task<IActionResult> GetUserProfile(string username)
		{
			return HandleResult(await Mediator.Send(new ProfileDetails.Query { Username = username }));
		}

		[HttpGet("{username}/activities")]
		public async Task<IActionResult> GetProfileActivities(string username, string predicate)
		{
			return HandleResult(await Mediator.Send(new ListActivityProfile.Query { Username = username, Predicate = predicate }));
		}

		[HttpPut]
		public async Task<IActionResult> Edit(EditProfile.Command command)
		{
			return HandleResult(await Mediator.Send(command));
		}

	}
}
