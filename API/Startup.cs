using API.Extensions;
using API.Middleware;
using API.SignalR;
using Application.Activities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// 
			services.AddControllers(opt =>
			{
				var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
				opt.Filters.Add(new AuthorizeFilter(policy));
			});

			// config validator
			services.AddControllers().AddFluentValidation(config =>
			{
				config.RegisterValidatorsFromAssemblyContaining<Create>();
			});

			// startup class houseKeeping! 
			// all services config bellow has been moved to AddAppServices file
			services.AddAppServices(Configuration);
			// add custom Identity Services config
			services.AddIdentityService(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// use custom exception middleware
			app.UseMiddleware<ExceptionMiddleware>();

			// add security headers for production mode
			// app.UseXContentTypeOptions();
			// app.UseReferrerPolicy(opt => opt.NoReferrer());
			// app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
			// app.UseXfo(opt => opt.Deny());
			// app.UseCspReportOnly(opt =>
			// {
			// 	opt.BlockAllMixedContent()
			// 	.StyleSources(s => s.Self())
			// 	.FontSources(s => s.Self())
			// 	.FormActions(s => s.Self())
			// 	.FrameAncestors(s => s.Self())
			// 	.ImageSources(s => s.Self())
			// 	.ScriptSources(s => s.Self());
			// });

			// in the production this will not be reached! 
			// so to handle exception, need to create exception middleware!
			if (env.IsDevelopment())
			{
				//  app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
			}

			// app.UseHttpsRedirection(); // comment this for API project
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = env.WebRootFileProvider
			});

			app.UseRouting();

			// configure CORs 2: use Cors
			app.UseCors("CorsPolicy");

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				// config signalR 2 
				endpoints.MapHub<ChatHub>("/chat");
				endpoints.MapFallbackToController("Index", "Fallback");
			});
		}
	}
}
