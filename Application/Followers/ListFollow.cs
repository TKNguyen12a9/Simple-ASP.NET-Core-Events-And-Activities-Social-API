using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Application.Profiles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
	public class ListFollow
	{
		public class Query : IRequest<Result<List<AttendeeProfile>>>
		{
			public string Predicate { get; set; }
			public string Username { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<AttendeeProfile>>>
		{
			private readonly AppDbContext _dbContext;
			private readonly IMapper _mapper;
			private readonly IUserAccessor _userAccessor;
			// private readonly IProfileReader _profileReader;

			public Handler(AppDbContext dbContext, IMapper mapper, IUserAccessor userAccessor)
			{
				_dbContext = dbContext;
				_mapper = mapper;
				_userAccessor = userAccessor;
			}

			public async Task<Result<List<AttendeeProfile>>> Handle(Query request, CancellationToken cancellationToken)
			{
				var profiles = new List<AttendeeProfile>();

				switch (request.Predicate)
				{
					case "followers":
						{
							profiles = await _dbContext.UserFollowings.Where(x => x.Target.UserName == request.Username)
								.Select(u => u.Observer)
								.ProjectTo<AttendeeProfile>(_mapper.ConfigurationProvider,
									// for detecing current logged in user is in followers list or not
									new { currentUsername = _userAccessor.GetUserName() })
								.ToListAsync();
							break;
						}

					case "followings":
						{
							profiles = await _dbContext.UserFollowings.Where(x => x.Observer.UserName == request.Username)
							.Select(x => x.Target)
							.ProjectTo<AttendeeProfile>(_mapper.ConfigurationProvider,
								new { currentUsername = _userAccessor.GetUserName() })
							.ToListAsync();
							break;

						}
				}

				return Result<List<AttendeeProfile>>.Success(profiles);
			}
		}
	}
}
