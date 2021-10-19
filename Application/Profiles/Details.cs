using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Persistence;

namespace Application.Profiles
{
    public class ProfileDetails
    {
        public class Query : IRequest<Result<AttendeeProfile>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AttendeeProfile>>
        {
            private readonly AppDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext dbContext, IMapper mapper, IUserAccessor userAccessor)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<AttendeeProfile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .ProjectTo<AttendeeProfile>(_mapper.ConfigurationProvider,
                        new { currentUsername = _userAccessor.GetUserName() }) // todo: read more about ProjectTo
                    .SingleOrDefaultAsync(x => x.Username == request.Username);

                if (user == null) return null;

                return Result<AttendeeProfile>.Success(user);
            }
        }
    }
}