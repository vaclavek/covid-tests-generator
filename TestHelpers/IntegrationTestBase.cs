using System;
using CTG.CovidTestsGenerator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CTG.CovidTestsGenerator.TestHelpers
{
	public class IntegrationTestBase
	{
		private IDisposable scope;

		protected IServiceProvider ServiceProvider { get; private set; }

		[TestInitialize]
		public virtual void TestInitialize()
		{
			IServiceCollection services = CreateServiceCollection();
			IServiceProvider serviceProvider = services.BuildServiceProvider();

			scope = serviceProvider.CreateScope();

			this.ServiceProvider = serviceProvider;
		}

		[TestCleanup]
		public virtual void TestCleanup()
		{
			scope.Dispose();
			if (this.ServiceProvider is IDisposable)
			{
				((IDisposable)this.ServiceProvider).Dispose();
			}
			this.ServiceProvider = null;
		}

		protected virtual IServiceCollection CreateServiceCollection()
		{
			IServiceCollection services = new ServiceCollection();
			services.ConfigureForTests();

			return services;
		}
	}
}
