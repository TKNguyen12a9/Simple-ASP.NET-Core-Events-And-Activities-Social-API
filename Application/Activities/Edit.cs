using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
   public class Edit
   {
      public class Command : IRequest
      {
         public Activity Activity { get; set; }
      }

      public class Handler : IRequestHandler<Command>
      {
         private readonly AppDbContext _context;
         private IMapper _mapper;
         public Handler(AppDbContext context, IMapper mapper)
         {
            _context = context;
            _mapper = mapper;
         }

         public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
         {
            var activityToUpate = await _context.Activities.FindAsync(request.Activity.Id);
            _mapper.Map(request.Activity, activityToUpate);
            // activityToUpate.Title = request.Activity.Title ?? activityToUpate.Title;
            await _context.SaveChangesAsync();
            return Unit.Value;
         }
      }
   }
}