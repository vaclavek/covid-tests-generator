using CTG.CovidTestsGenerator.Contracts;
using CTG.CovidTestsGenerator.Contracts.System;
using CTG.CovidTestsGenerator.DependencyInjection;
using CTG.CovidTestsGenerator.Web.Server.Infrastructure.ApplicationInsights;
using CTG.CovidTestsGenerator.Web.Server.Infrastructure.ConfigurationExtensions;
using CTG.CovidTestsGenerator.Web.Server.Infrastructure.HealthChecks;
using Havit.Blazor.Grpc.Server;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CTG.CovidTestsGenerator.Web.Server
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureForWebServer(configuration);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddOptions();

			services.AddCustomizedMailing(configuration);

			// SmtpExceptionMonitoring to errors@havit.cz
			services.AddExceptionMonitoring(configuration);

			// Application Insights
			services.AddApplicationInsightsTelemetry(configuration);
			services.AddSingleton<ITelemetryInitializer, GrpcRequestStatusTelemetryInitializer>();
			services.AddSingleton<ITelemetryInitializer, EnrichmentTelemetryInitializer>();
			services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

			// server-side UI
			services.AddControllersWithViews();
			services.AddRazorPages();

			// gRPC
			services.AddGrpcServerInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);

			// Health checks
			services.AddHealthChecks();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				// TODO app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseExceptionMonitoring();

			app.UseRouting();

			app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");

				endpoints.MapGrpcServicesByApiContractAttributes(typeof(IMaintenanceFacade).Assembly);

				endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
				{
					AllowCachingResponses = false,
					ResponseWriter = HealthCheckWriter.WriteResponse
				});
			});
		}
	}
}
