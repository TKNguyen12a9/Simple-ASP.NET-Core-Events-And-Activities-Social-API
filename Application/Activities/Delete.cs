using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
   public class Delete
   {
      // Command # Query: Command does not return anything!!
      public class Command : IRequest
      {
         public Guid Id { get; set; }
      }

      public class Handler : IRequestHandler<Command>
      {
         private readonly AppDbContext _context;
         public Handler(AppDbContext context)
         {
            _context = context;
         }

         public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
         {
            var activityToDelete = await _context.Activities.FindAsync(request.Id);
            _context.Activities.Remove(activityToDelete);
            await _context.SaveChangesAsync();
            return Unit.Value;
         }
      }
   }
}