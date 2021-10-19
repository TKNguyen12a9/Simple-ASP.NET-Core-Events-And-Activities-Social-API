using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
   public class Edit
   {
      public class Command : IRequest<Result<Unit>>
      {
         public Activity Activity { get; set; }
      }

      public class Handler : IRequestHandler<Command, Result<Unit>>
      {
         private readonly AppDbContext _context;
         private IMapper _mapper;
         public Handler(AppDbContext context, IMapper mapper)
         {
            _context = context;
            _mapper = mapper;
         }

         public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
         {
            var activityToUpate = await _context.Activities.FindAsync(request.Activity.Id);
            if (activityToUpate == null) return null;

            _mapper.Map(request.Activity, activityToUpate);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to edit activity.");
            return Result<Unit>.Success(Unit.Value);
         }
      }
   }
}