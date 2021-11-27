using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Persistence;

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
				var activity = await _context.Activities
				   .ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider,
						new { currentUsername = _userAccessor.GetUserName() })
				   .FirstOrDefaultAsync(x => x.Id == request.Id);

				if (activity == null) return null;

				return Result<ActivityDTO>.Success(activity);
			}
		}
	}
}
