using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts;
using CTG.CovidTestsGenerator.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CTG.CovidTestsGenerator.Web.Client.Pages
{
	public partial class New
	{
		private readonly UserData model = new();

		private readonly List<TestType> testTypes = new()
		{
			new TestType { Value = false, Title = "Negativní" },
			new TestType { Value = true, Title = "Pozitivní" }
		};

		[Inject] public IJSRuntime JsRuntime { get; set; }
		[Inject] public IPdfGenerator PdfGenerator { get; set; }

		protected async Task GenerateAsync()
		{
			var data = await PdfGenerator.GetPdfAsync(model);

			await JsRuntime.InvokeVoidAsync("jsSaveAsFile", $"Test-{model.TestDateTime.Value:yyyyMMdd}.pdf", Convert.ToBase64String(data));
		}
	}

	internal class TestType
	{
		public bool Value { get; set; }
		public string Title { get; set; }
	}
}
