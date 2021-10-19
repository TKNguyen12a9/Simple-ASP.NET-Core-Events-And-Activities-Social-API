using System;
using MediatR;
using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Persistence;
using Microsoft.Extensions.Logging;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Interfaces;
using System.Linq;

namespace Application.Activities
{
	public class List
	{
		public class Query : IRequest<Result<PagedList<ActivityDTO>>>
		{
			public ActivityParams Params { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDTO>>>
		{
			private readonly AppDbContext _context;
			private readonly ILogger<List> _logger;
			private readonly IMapper _mapper;
			private readonly IUserAccessor _userAccessor;

			public Handler(AppDbContext context, ILogger<List> logger, IMapper mapper, IUserAccessor userAccessor)
			{
				_context = context;
				_logger = logger;
				_mapper = mapper;
				_userAccessor = userAccessor;
			}

			public async Task<Result<PagedList<ActivityDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				// using traditional related data loading => not optimized
				// var activities = await _context.Activities
				//  .Include(x => x.Attendees)
				//  .ThenInclude(y => y.Activity)
				//  .ToListAsync(cancellationToken);

				// var activitiesToReturn = _mapper.Map<List<ActivityDTO>>(activities);

				// using ProjectTo => optimized and short
				var query = _context.Activities
					.Where(x => x.Date >= request.Params.StartDate) // dateTime filter
					.OrderBy(x => x.Date) // order by date
					.ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider,
						new { currentUsername = _userAccessor.GetUserName() })
					.AsQueryable();

				// isGoing filter
				if (request.Params.IsGoging && !request.Params.IsHosting)
				{
					var newQuery = query.Where(x => x.Attendees.Any(y => y.Username == _userAccessor.GetUserName()));
					query = newQuery;
				}

				// isHost filter
				if (request.Params.IsHosting && !request.Params.IsGoging)
				{
					query = query.Where(x => x.HostUsername == _userAccessor.GetUserName());
				}

				return Result<PagedList<ActivityDTO>>.Success(
					await PagedList<ActivityDTO>.CreateAsync(query, request.Params.PageNumber,
						request.Params.PageSize)
				);
			}
		}
	}
}