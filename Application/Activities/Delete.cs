using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
	public class Delete
	{
		// Command # Query: Command does not return anything!!
		public class Command : IRequest<Result<Unit>>
		{
			public Guid Id { get; set; }
		}


		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly AppDbContext _context;
			public Handler(AppDbContext context)
			{
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				var activityToDelete = await _context.Activities.FindAsync(request.Id);

				_context.Remove(activityToDelete);

				var result = await _context.SaveChangesAsync() > 0;

				if (!result) return Result<Unit>.Failure("Failed to delete activity.");

				return Result<Unit>.Success(Unit.Value);
			}
		}
	}
}
