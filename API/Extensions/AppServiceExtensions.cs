using System.Collections.Generic;
using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
   // Startup houseKeeping
   public static class AppServiceExtensions
   {
      public static IServiceCollection AddAppServices(this IServiceCollection services,
          IConfiguration configuration)
      {
         services.AddSwaggerGen(c =>
         {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
         });

         services.AddDbContext<AppDbContext>(opt =>
         {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
         });

         // configure CORs 1: for calling API from client
         services.AddCors(opt =>
         {
            opt.AddPolicy("CorsPolicy", policy =>
            {
               policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000"); // client host 
            });
         });

         // add mediator
         services.AddMediatR(typeof(List.Handler).Assembly);
         // autoMapper
         services.AddAutoMapper(typeof(MappingProfiles).Assembly);

         return services;
      }
   }
}