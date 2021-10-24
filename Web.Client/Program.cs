using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using CTG.CovidTestsGenerator.Contracts;
using CTG.CovidTestsGenerator.Contracts.System;
using CTG.CovidTestsGenerator.Web.Client.Infrastructure.Grpc;
using CTG.CovidTestsGenerator.Web.Client.Services;
using FluentValidation;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Grpc.Client;
using Havit.Blazor.Grpc.Client.ServerExceptions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CTG.CovidTestsGenerator.Web.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);

			AddLoggingAndApplicationInsights(builder);

			builder.RootComponents.Add<App>("app");

			builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			builder.Services.AddBlazoredLocalStorage();
			builder.Services.AddValidatorsFromAssemblyContaining<Dto<object>>();

			builder.Services.AddHxServices();
			builder.Services.AddHxMessenger();
			builder.Services.AddHxMessageBoxHost();
			CTG.CovidTestsGenerator.Web.Client.Resources.ResourcesServiceCollectionInstaller.AddGeneratedResourceWrappers(builder.Services);
			CTG.CovidTestsGenerator.Resources.ResourcesServiceCollectionInstaller.AddGeneratedResourceWrappers(builder.Services);
			SetHxComponents();

			AddGrpcClient(builder);

			builder.Services.AddScoped<IPdfGenerator, PdfGenerator>();

			WebAssemblyHost webAssemblyHost = builder.Build();

			await SetLanguage(webAssemblyHost);

			await webAssemblyHost.RunAsync();
		}
		private static void SetHxComponents()
		{
			// HxProgressIndicator.DefaultDelay = 0;
		}

		private static void AddGrpcClient(WebAssemblyHostBuilder builder)
		{
			builder.Services.AddTransient<IOperationFailedExceptionGrpcClientListener, HxMessengerOperationFailedExceptionGrpcClientListener>();
			builder.Services.AddGrpcClientInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);
			builder.Services.AddGrpcClientsByApiContractAttributes(typeof(IMaintenanceFacade).Assembly);
		}

		private static async ValueTask SetLanguage(WebAssemblyHost webAssemblyHost)
		{
			var localStorageService = webAssemblyHost.Services.GetService<ILocalStorageService>();

			var culture = await localStorageService.GetItemAsStringAsync("culture");
			if (!String.IsNullOrWhiteSpace(culture))
			{
				var cultureInfo = new CultureInfo(culture);
				CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
				CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
			}
		}

		private static void AddLoggingAndApplicationInsights(WebAssemblyHostBuilder builder)
		{
			var instrumentationKey = builder.Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

			builder.Services.AddBlazorApplicationInsights(async applicationInsights =>
			{
				await applicationInsights.SetInstrumentationKey(instrumentationKey);
				await applicationInsights.LoadAppInsights();

				var telemetryItem = new TelemetryItem()
				{
					Tags = new Dictionary<string, object>()
					{
						{ "ai.cloud.role", "Web.Client" },
						// { "ai.cloud.roleInstance", "..." },
					}
				};

				await applicationInsights.AddTelemetryInitializer(telemetryItem);
			}, addILoggerProvider: true);

			builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(level => (level == LogLevel.Error) || (level == LogLevel.Critical));

#if DEBUG
			builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
		}
	}
}
