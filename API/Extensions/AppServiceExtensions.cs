using System.Collections.Generic;
using Application.Activities;
using Application.Interfaces;
using Infrastructure.Security;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;
using Microsoft.Extensions.Logging;
using Infrastructure.Photos;
using Application.Profiles;

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
                    policy.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() // resolve connecting error (CORs policy) of signalR on client 
                        .WithOrigins("http://localhost:3000"); // client host 
                });
            });

            // config signalR 1 
            services.AddSignalR(opt =>
            {
                opt.EnableDetailedErrors = true;
            });


            // add mediator
            services.AddMediatR(typeof(List.Handler).Assembly);

            // autoMapper
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            // to get current logged-in userName in everywhere in application
            services.AddScoped<IUserAccessor, UserAccessor>();

            // scope for photo accessor 
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            // services.AddScoped<IProfileReader, ProfileReader>();

            // log efcore db config, queries, etc.. 
            services.AddDbContext<DbContext>(opt =>
            {
                opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                opt.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));
            });

            // config for Cloudinary
            services.Configure<CloudinaryConfig>(configuration.GetSection("Cloudinary"));

            return services;
        }
    }
}