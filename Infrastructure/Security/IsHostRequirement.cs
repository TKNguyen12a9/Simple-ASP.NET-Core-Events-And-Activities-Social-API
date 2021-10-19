using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistence;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


// helper module for EDITING activity as host
// todo: read more about this helper class 
namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {

    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IsHostRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var currentContextId = _contextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString();

            var activityId = Guid.Parse(currentContextId);

            // var activityId = Guid.Parse(_contextAccessor.HttpContext?.Request.RouteValues
            //     .SingleOrDefault(x => x.Key == "id").Value?.ToString());

            /*this will caused then policy error(host and guest both are be able to edit activity)*/
            // var attendee = _dbContext.ActivityAttendees.Find(activityId, userId);

            /*this will make sure only host will be able to edit activity*/
            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking()
                .FirstOrDefault(x => x.AppUserId == userId && x.ActivityId == activityId);

            if (attendee == null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}