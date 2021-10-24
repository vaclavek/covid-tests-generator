using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts;
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
		private readonly PdfFont font;

		public PdfGenerator(HttpClient httpClient)
		{
			this.httpClient = httpClient;
			font = PdfFontFactory.CreateRegisteredFont("helvetica", "windows-1250");
		}

		public async Task<byte[]> GetPdfAsync(UserData userData)
		{
			// Marge in centimeter, then I convert with .ToDpi()
			//float margeLeft = 1.5f;
			//float margeRight = 1.5f;
			//float margeTop = 1.0f;
			//float margeBottom = 1.0f;

			using var streamWithEmptyPdf = await httpClient.GetStreamAsync("/empty.pdf");
			using var reader = new PdfReader(streamWithEmptyPdf);
			using var stream = new MemoryStream();
			using var writer = new PdfWriter(stream);
			using var pdf = new PdfDocument(reader, writer);
			using var document = new Document(pdf);

			AddFullName(document, userData.FullName);
			AddPernamentAddress(document, userData.PernamentAddress);
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
			var paragraph = new Paragraph(fullName);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 685, 1000);
			document.Add(paragraph);
		}

		private void AddPernamentAddress(Document document, string address)
		{
			var paragraph = new Paragraph(address);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 638, 1000);
			document.Add(paragraph);

		}

		private void AddCurrentAddress(Document document, string address)
		{
			var paragraph = new Paragraph(address);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 592, 1000);
			document.Add(paragraph);
		}

		private void AddDateOfBirth(Document document, DateTime dateOfBirth)
		{
			var paragraph = new Paragraph(dateOfBirth.ToString("dd. MM. yyyy"));
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 542, 1000);
			document.Add(paragraph);
		}

		private void AddPhoneNumber(Document document, string phoneNumber)
		{
			var paragraph = new Paragraph(phoneNumber);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(308, 542, 1000);
			document.Add(paragraph);
		}

		private void AddEmail(Document document, string email)
		{
			var paragraph = new Paragraph(email);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 506, 1000);
			document.Add(paragraph);
		}

		private void AddPassportOrIdNumber(Document document, string passportOrIdNumber)
		{
			var paragraph = new Paragraph(passportOrIdNumber);
			paragraph.SetFont(font);
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
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 360, 1000);
			document.Add(paragraph);


			var paragraphDate = new Paragraph(testDateTime.Value.ToString("dd. MM. yyyy"));
			paragraphDate.SetFont(font);
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
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 400, 1000);
			document.Add(paragraph);
		}

		private void AddTestPlace(Document document, string testPlace)
		{
			var paragraph = new Paragraph(testPlace);
			paragraph.SetFont(font);
			paragraph.SetFixedPosition(100, 317, 1000);
			document.Add(paragraph);
		}
	}
}
