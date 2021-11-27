using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public class ListActivityProfile
	{
		public class Query : IRequest<Result<List<UserActivityProfileDTO>>>
		{
			public string Predicate { get; set; }
			public string Username { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<UserActivityProfileDTO>>>
		{
			private readonly AppDbContext _dbContext;
			private readonly IMapper _mapper;

			public Handler(AppDbContext dbContext, IMapper mapper)
			{
				_dbContext = dbContext;
				_mapper = mapper;
			}

			public async Task<Result<List<UserActivityProfileDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				var query = _dbContext.ActivityAttendees
					.Where(x => x.ApplicationUser.UserName == request.Username)
					.OrderBy(x => x.Activity.Date)
					.ProjectTo<UserActivityProfileDTO>(_mapper.ConfigurationProvider)
					.AsQueryable();
				
				query = request.Predicate switch
				{
					"past" => query.Where(x => x.Date < DateTime.UtcNow),
					"hosting" => query.Where(x => x.HostUsername == request.Username),
					_ => query.Where(x => x.Date > DateTime.UtcNow)
				};

				return Result<List<UserActivityProfileDTO>>.Success(await query.ToListAsync());
			}
		}
	}
}
