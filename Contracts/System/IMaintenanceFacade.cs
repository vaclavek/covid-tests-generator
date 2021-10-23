using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Havit.ComponentModel;

namespace CTG.CovidTestsGenerator.Contracts.System
{
	[ApiContract]
	public interface IMaintenanceFacade
	{
		Task ClearCache(CancellationToken cancellationToken = default);
	}
}