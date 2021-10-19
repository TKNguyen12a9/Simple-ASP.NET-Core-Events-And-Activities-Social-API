using System;
using MediatR;
using Domain;
using System.Threading.Tasks;
using System.Threading;
using Persistence;
using Application.Core;
using Microsoft.Extensions.FileProviders;
using System.Reflection.Metadata.Ecma335;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Application.Interfaces;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<Result<ActivityDTO>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActivityDTO>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<ActivityDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                //  var activity = await _context.Activities.FindAsync(request.Id);

                var activity = await _context.Activities
                   .ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider,
                        new { currentUsername = _userAccessor.GetUserName() })
                   .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null)
                {
                    return null;
                    // return Result<Activity>.Failure("activity cound not be found.");
                }

                return Result<ActivityDTO>.Success(activity);
            }
        }
    }
}