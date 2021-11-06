using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.Model;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using static iText.Kernel.Font.PdfFontFactory;

namespace CTG.CovidTestsGenerator.Web.Client.Services
{

	public class PdfGenerator : IPdfGenerator
	{
		private readonly HttpClient httpClient;

		public PdfGenerator(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task<byte[]> GetPdfAsync(UserData userData)
		{
			PdfFont font = PdfFontFactory.CreateRegisteredFont("helvetica", "windows-1250");

			using var streamWithEmptyPdf = await httpClient.GetStreamAsync("/empty.pdf");
			using var reader = new PdfReader(streamWithEmptyPdf);
			using var stream = new MemoryStream();
			using var writer = new PdfWriter(stream);
			using var pdf = new PdfDocument(reader, writer);
			using var document = new Document(pdf);
			document.SetFont(font);

			AddFullName(document, userData.FullName);
			AddPernamentAddress(document, userData.PermanentAddress);
			AddCurrentAddress(document, userData.CurrentAddress);
			AddDateOfBirth(document, userData.DateOfBirth.Value);
			AddPhoneNumber(document, userData.PhoneNumber);
			AddEmail(document, userData.Email);
			AddPassportOrIdNumber(document, userData.PassportOrIdNumber);
			AddTestDateTime(document, userData.TestDateTime);
			AddTestType(document, userData.TestType);
			AddTestPlace(document, userData.TestPlace);
			AddTestName(document);

			document.Close();
			return stream.ToArray();
		}

		private void AddFullName(Document document, string fullName)
		{
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return;
			}

			var paragraph = new Paragraph(fullName);
			paragraph.SetFixedPosition(100, 685, 1000);
			document.Add(paragraph);
		}

		private void AddPernamentAddress(Document document, string address)
		{
			if (string.IsNullOrWhiteSpace(address))
			{
				return;
			}

			var paragraph = new Paragraph(address);
			paragraph.SetFixedPosition(100, 638, 1000);
			document.Add(paragraph);
		}

		private void AddCurrentAddress(Document document, string address)
		{
			if (string.IsNullOrWhiteSpace(address))
			{
				return;
			}

			var paragraph = new Paragraph(address);
			paragraph.SetFixedPosition(100, 592, 1000);
			document.Add(paragraph);
		}

		private void AddDateOfBirth(Document document, DateTime dateOfBirth)
		{
			var paragraph = new Paragraph(dateOfBirth.ToString("dd. MM. yyyy"));
			paragraph.SetFixedPosition(100, 542, 1000);
			document.Add(paragraph);
		}

		private void AddPhoneNumber(Document document, string phoneNumber)
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
			{
				return;
			}

			var paragraph = new Paragraph(phoneNumber);
			paragraph.SetFixedPosition(308, 542, 1000);
			document.Add(paragraph);
		}

		private void AddEmail(Document document, string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return;
			}

			var paragraph = new Paragraph(email);
			paragraph.SetFixedPosition(100, 506, 1000);
			document.Add(paragraph);
		}

		private void AddPassportOrIdNumber(Document document, string passportOrIdNumber)
		{
			if (string.IsNullOrWhiteSpace(passportOrIdNumber))
			{
				return;
			}

			var paragraph = new Paragraph(passportOrIdNumber);
			paragraph.SetFixedPosition(308, 506, 1000);
			document.Add(paragraph);
		}

		private void AddTestDateTime(Document document, DateTime? testDateTime)
		{
			if (testDateTime == null)
			{
				return;
			}

			var paragraph = new Paragraph(testDateTime.Value.ToString("dd. MM. yyyy HH:mm:ss"));
			paragraph.SetFixedPosition(100, 360, 1000);
			document.Add(paragraph);

			var paragraphDate = new Paragraph(testDateTime.Value.ToString("dd. MM. yyyy"));
			paragraphDate.SetFixedPosition(100, 165, 1000);
			document.Add(paragraphDate);
		}

		private void AddTestType(Document document, bool testType)
		{
			string text;
			if (testType)
			{
				text = "Positive * / Positiv * / Pozitivní *";
			}
			else
			{
				text = "Negative * / Negativ * / Negativní *";
			}

			var boldFont = PdfFontFactory.CreateRegisteredFont("helvetica", "windows-1250", EmbeddingStrategy.PREFER_EMBEDDED, iText.IO.Font.Constants.FontStyles.BOLD);
			var paragraph = new Paragraph(text);
			paragraph.SetFont(boldFont);
			paragraph.SetFontColor(new DeviceRgb(3, 2, 128));
			paragraph.SetFixedPosition(170, 234, 1000);
			document.Add(paragraph);
		}

		private void AddTestName(Document document)
		{
			string testName = "VivaDiag Pro SARS CoV 2 Ag Rapid test, VivaChek Biotech (Hangzhou) Co., Ltd.";
			var paragraph = new Paragraph(testName);
			paragraph.SetFixedPosition(100, 400, 1000);
			document.Add(paragraph);
		}

		private void AddTestPlace(Document document, string testPlace)
		{
			if (string.IsNullOrWhiteSpace(testPlace))
			{
				return;
			}

			var paragraph = new Paragraph(testPlace);
			paragraph.SetFixedPosition(100, 317, 1000);
			document.Add(paragraph);
		}
	}
}
