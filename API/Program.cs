using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Writers;
using Persistence;

namespace API
{
   public class Program
   {

      // refer another this config: https://github.com/TKNguyen12a9/ASP.NET-Core-Simple-School-API/blob/master/src/SchoolApiSrc/Program.cs
      public static async Task Main(string[] args)
      {
         var host = CreateHostBuilder(args).Build();
         using var scope = host.Services.CreateScope();
         var services = scope.ServiceProvider;

         try
         {
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            await SeedDB.Seed(context);
         }
         catch (Exception e)
         {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(e, "An error occured while creating Db.");
         }

         await host.RunAsync();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                 webBuilder.UseStartup<Startup>();
              });
   }
}
