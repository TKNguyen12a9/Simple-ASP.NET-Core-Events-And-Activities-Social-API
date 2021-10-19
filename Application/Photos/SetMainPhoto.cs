using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMainPhoto
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
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
                var user = await _dbContext.Users
                    .Include(x => x.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                if (user == null) return null;

                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                if (photo == null) return null;

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                // if current photo is main => !main
                if (currentMain != null) currentMain.IsMain = false;
                // if not => main
                photo.IsMain = true;

                var result = await _dbContext.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Failed while setting photo.");
            }
        }
    }
}
