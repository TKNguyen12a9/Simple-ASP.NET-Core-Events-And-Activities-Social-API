using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Persistence;
using Persistence.Migrations;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetUsername { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _dbContext;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext dbContext, IUserAccessor userAccessor)
            {
                _dbContext = dbContext;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var observer = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                var target = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.TargetUsername);

                if (target == null) return null;

                var following = await _dbContext.UserFollowings.FindAsync(observer.Id, target.Id);

                if (following == null)
                {
                    following = new UserFollowing
                    {
                        Target = target,
                        Observer = observer
                    };

                    _dbContext.UserFollowings.Add(following);
                }
                else
                {
                    _dbContext.UserFollowings.Remove(following);
                }

                var result = await _dbContext.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Failed while updating following.");
            }
        }
    }
}