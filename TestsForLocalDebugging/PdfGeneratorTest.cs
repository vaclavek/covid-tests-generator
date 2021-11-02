using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.Model;
using CTG.CovidTestsGenerator.Web.Client.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CTG.CovidTestsGenerator.TestsForLocalDebugging
{
	[TestClass]
	public class PdfGeneratorTest
	{
		[TestMethod]
		[Ignore]
		public async Task TestAsync()
		{
			var userData = new UserData
			{
				FullName = "Jan Novák",
				PermanentAddress = "Brněnská 123, 120 00 Praha - Nové Město",
				CurrentAddress = "Pražské předměstí 123, 621 00 Brno - Staré Město",
				DateOfBirth = new DateTime(2000, 12, 31),
				Email = "toto.je.muj@email.cz",
				PassportOrIdNumber = "231 451 321",
				PhoneNumber = "+420 777 657 954",
				TestDate = new DateTime(2021, 10, 24, 12, 13, 14),
				TestHour = 16,
				TestMinute = 40,
				TestType = false
			};

			var httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://localhost:44301/")
			};
			var pdf = new PdfGenerator(httpClient);
			var data = await pdf.GetPdfAsync(userData);

			await File.WriteAllBytesAsync("test.pdf", data);
		}
	}
}
