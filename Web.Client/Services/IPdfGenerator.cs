using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.Model;

namespace CTG.CovidTestsGenerator.Web.Client.Services
{
	public interface IPdfGenerator
	{
		Task<byte[]> GetPdfAsync(UserData userData);
	}
}