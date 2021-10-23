using System.Threading;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.System;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Services.Caching;

namespace CTG.CovidTestsGenerator.Facades.System
{
	[Service]
	public class MaintenanceFacade : IMaintenanceFacade
	{
		private readonly ICacheService cacheService;

		public MaintenanceFacade(ICacheService cacheService)
		{
			this.cacheService = cacheService;
		}

		public Task ClearCache(CancellationToken cancellationToken = default)
		{
			cacheService.Clear();

			return Task.CompletedTask;
		}
	}
}
