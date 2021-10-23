using System;
using System.Threading;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.DependencyInjection;
using CTG.CovidTestsGenerator.Services.Jobs;
using Hangfire.Console.Extensions;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CTG.CovidTestsGenerator.Utility
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			if (args.Length > 0)
			{
				string command = args[0];

				Console.WriteLine($"Command: {command}");

				bool successfullyCompleted =
					await TryDoCommand<IEmptyJob>(command, "EmptyJob");

				if (!successfullyCompleted)
				{
					Console.WriteLine("Nepodařilo se zpracovat příkaz: {0}", command);
					Console.WriteLine();

					ShowCommandsHelp();
				}
			}
		}

		private static void ShowCommandsHelp()
		{
			Console.WriteLine("Podporované příkazy jsou:");
			Console.WriteLine("  EmptyJob");
		}

		private static async Task<bool> TryDoCommand<TJob>(string command, string commandPattern)
			where TJob : class, IRunnableJob
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(command));
			Contract.Requires<ArgumentNullException>(commandPattern != null);

			if (!String.Equals(command, commandPattern, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			await ExecuteWithServiceProvider(async serviceProvider =>
			{
				try
				{
					using (var scopeService = serviceProvider.CreateScope())
					{
						TJob job = scopeService.ServiceProvider.GetRequiredService<TJob>();
						await job.ExecuteAsync(CancellationToken.None);
					}
				}
				catch (Exception ex)
				{
					var service = serviceProvider.GetRequiredService<IExceptionMonitoringService>();
					service.HandleException(ex);

					throw;
				}
			});

			return true;
		}

		private static async Task ExecuteWithServiceProvider(Func<IServiceProvider, Task> action)
		{
			IConfiguration configuration = Configuration.Value;

			// Setup ServiceCollection
			IServiceCollection services = new ServiceCollection();

			services.AddOptions();
			services.AddMemoryCache();

			services.AddLogging(builder => builder.AddSimpleConsole(options => options.TimestampFormat = "[HH:mm:ss] "));
			services.AddHangfireConsoleExtensions(); // adds support for Hangfire jobs logging  to a dashboard using ILogger<T> (.UseConsole() in hangfire configuration is required!)
			services.AddExceptionMonitoring(configuration);

			services.ConfigureForUtility(configuration);

			services.AddSingleton<TelemetryClient>();

			using (ServiceProvider serviceProvider = services.BuildServiceProvider())
			{
				await action(serviceProvider);
			}
		}

		private static readonly Lazy<IConfiguration> Configuration = new Lazy<IConfiguration>(() =>
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile(@"appsettings.Utility.json", optional: false)
				.AddJsonFile($"appsettings.Utility.{environmentName}.json", optional: true)
				.AddEnvironmentVariables();

			return configurationBuilder.Build();
		});
	}
}
