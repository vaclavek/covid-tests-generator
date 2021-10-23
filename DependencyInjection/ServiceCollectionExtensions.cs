using System;
using System.IO;
using System.Runtime.CompilerServices;
using Azure.Identity;
using CTG.CovidTestsGenerator.DependencyInjection.ConfigrationOptions;
using CTG.CovidTestsGenerator.Services.Infrastructure;
using CTG.CovidTestsGenerator.Services.Infrastructure.FileStorages;
using CTG.CovidTestsGenerator.Services.Jobs;
using CTG.CovidTestsGenerator.Services.TimeServices;
using Havit.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Caching;
using Havit.Services.FileStorage;
using Havit.Services.TimeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTG.CovidTestsGenerator.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static IServiceCollection ConfigureForWebServer(this IServiceCollection services, IConfiguration configuration)
		{
			FileStorageOptions fileStorageOptions = new FileStorageOptions();
			configuration.GetSection(FileStorageOptions.FileStorageOptionsKey).Bind(fileStorageOptions);

			InstallConfiguration installConfiguration = new InstallConfiguration
			{
				DatabaseConnectionString = configuration.GetConnectionString("Database"),
				AzureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString"),
				FileStoragePathOrContainerName = fileStorageOptions.PathOrContainerName,
				ServiceProfiles = new[] { ServiceAttribute.DefaultProfile, ServiceProfiles.WebServer },
			};

			return services.ConfigureForAll(installConfiguration);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static IServiceCollection ConfigureForUtility(this IServiceCollection services, IConfiguration configuration)
		{
			InstallConfiguration installConfiguration = new InstallConfiguration
			{
				DatabaseConnectionString = configuration.GetConnectionString("Database"),
				AzureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString"),
				ServiceProfiles = new[] { ServiceAttribute.DefaultProfile, Jobs.ProfileName },
				ApiCommunicationLogStorage = configuration.GetValue<string>("AppSettings:ApiCommunicationLogStorage:Path")
			};

			return services.ConfigureForAll(installConfiguration);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static IServiceCollection ConfigureForTests(this IServiceCollection services)
		{
			string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Developement";

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{environment}.json", true)
				.Build();

			InstallConfiguration installConfiguration = new InstallConfiguration
			{
				DatabaseConnectionString = configuration.GetConnectionString("Database"),
				ServiceProfiles = new[] { ServiceAttribute.DefaultProfile },
				UseInMemoryDb = true,
			};

			return services.ConfigureForAll(installConfiguration);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IServiceCollection ConfigureForAll(this IServiceCollection services, InstallConfiguration installConfiguration)
		{
			InstallHavitServices(services);
			InstallByServiceAttribute(services, installConfiguration);
			InstallAuthorizationHandlers(services);
			InstallFileServices(services, installConfiguration);

			services.AddMemoryCache();

			return services;
		}

		private static void InstallHavitServices(IServiceCollection services)
		{
			//  .NET Framework Extensions
			services.AddSingleton<ITimeService, ApplicationTimeService>();
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddSingleton(new MemoryCacheServiceOptions { UseCacheDependenciesSupport = false });
		}

		private static void InstallByServiceAttribute(IServiceCollection services, InstallConfiguration configuration)
		{
			services.AddByServiceAttribute(typeof(CTG.CovidTestsGenerator.Services.Properties.AssemblyInfo).Assembly, configuration.ServiceProfiles);
			services.AddByServiceAttribute(typeof(CTG.CovidTestsGenerator.Facades.Properties.AssemblyInfo).Assembly, configuration.ServiceProfiles);
		}

		private static void InstallAuthorizationHandlers(IServiceCollection services)
		{
			services.Scan(scan => scan.FromAssemblyOf<CTG.CovidTestsGenerator.Services.Properties.AssemblyInfo>()
				.AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
				.As<IAuthorizationHandler>()
				.WithScopedLifetime()
			);
		}

		private static void InstallFileServices(IServiceCollection services, InstallConfiguration installConfiguration)
		{
			InstallFileStorageService<IApplicationFileStorageService, ApplicationFileStorageService, ApplicationFileStorage>(services, installConfiguration.AzureStorageConnectionString, installConfiguration.FileStoragePathOrContainerName);
		}

		internal static void InstallFileStorageService<TFileStorageService, TFileStorageImplementation, TFileStorageContext>(IServiceCollection services, string azureStorageConnectionString, string storagePath)
			where TFileStorageService : class, IFileStorageService<TFileStorageContext> // class zde znamená i interface! // např. IDocumentStorageService
			where TFileStorageImplementation : FileStorageWrappingService<TFileStorageContext>, TFileStorageService // např. DocumentStorageService
			where TFileStorageContext : FileStorageContext // např. DocumentStorage
		{
			services.AddFileStorageWrappingService<TFileStorageService, TFileStorageImplementation, TFileStorageContext>();

			if (!String.IsNullOrEmpty(azureStorageConnectionString))
			{
				services.AddAzureBlobStorageService<TFileStorageContext>(new AzureBlobStorageServiceOptions<TFileStorageContext>
				{
					BlobStorage = azureStorageConnectionString,
					ContainerName = storagePath,
					TokenCredential = new DefaultAzureCredential()
				});
			}
			else
			{
				services.AddFileSystemStorageService<TFileStorageContext>(storagePath);
			}
		}
	}
}