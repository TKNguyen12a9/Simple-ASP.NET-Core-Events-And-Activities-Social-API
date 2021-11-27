using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace Application.Profiles
{
	public class EditProfile
	{
		public class Command : IRequest<Result<Unit>>
		{
			public string DisplayName { get; set; }
			public string Bio { get; set; }
		}

		// form fields validators
		public class CommandVadidator : AbstractValidator<Command>
		{
			public CommandVadidator()
			{
				RuleFor(x => x.DisplayName).NotEmpty();
			}
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
				var profileToUpdate = await _dbContext.Users
					.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

				if (profileToUpdate == null) return null;

				// handle newly updated values are same or null
				profileToUpdate.DisplayName = request.DisplayName ?? profileToUpdate.DisplayName;
				profileToUpdate.Bio = request.Bio ?? profileToUpdate.Bio;

				var result = await _dbContext.SaveChangesAsync() > 0;

				if (!result) return Result<Unit>.Failure("Failed while editing profile.");
				return Result<Unit>.Success(Unit.Value);
			}
		}
	}
}
