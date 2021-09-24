using System;
using MediatR;
using Domain;
using System.Collections.Generic;
using Microsoft.Data.SqlClient.DataClassification;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection.Metadata;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Application.Activities
{
   public class List
   {
      public class Query : IRequest<List<Activity>>
      {
      }

      public class Handler : IRequestHandler<Query, List<Activity>>
      {
         private readonly AppDbContext _context;

         // Test cancellToken
         private readonly ILogger<List> _logger;
         public Handler(AppDbContext context, ILogger<List> logger)
         {
            _context = context;
            _logger = logger;
         }

         public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
         {
            // Testing cancellToken for certain running task
            // try
            // {
            //    for (var i = 0; i < 10; ++i)
            //    {
            //       cancellationToken.ThrowIfCancellationRequested();
            //       await Task.Delay(1000, cancellationToken);
            //       _logger.LogInformation($"Task {i} has completed");
            //    }
            // }
            // catch (Exception ex) when (ex is TaskCanceledException)
            // {
            //    _logger.LogInformation($"Task was cancelled");
            // }
            return await _context.Activities.ToListAsync(cancellationToken);
         }
      }
   }
}