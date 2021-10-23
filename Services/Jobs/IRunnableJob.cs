using System.Threading;
using System.Threading.Tasks;

namespace CTG.CovidTestsGenerator.Services.Jobs
{
	public interface IRunnableJob
	{
		Task ExecuteAsync(CancellationToken cancellationToken);
	}
}
