using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTG.CovidTestsGenerator.Web.Server.Infrastructure.ConfigurationExtensions
{
	public static class MailingConfig
	{
		public static void AddCustomizedMailing(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<MailingOptions>(configuration.GetSection("AppSettings:MailingOptions"));
		}
	}
}
