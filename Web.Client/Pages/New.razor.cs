using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CTG.CovidTestsGenerator.Contracts;
using CTG.CovidTestsGenerator.Web.Client.Resources.Pages;
using CTG.CovidTestsGenerator.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CTG.CovidTestsGenerator.Web.Client.Pages
{
	public partial class New
	{
		private UserData model = new();

		private readonly List<TestType> testTypes = new();

		[Inject] public IJSRuntime JsRuntime { get; set; }
		[Inject] public IPdfGenerator PdfGenerator { get; set; }
		[Inject] protected INewLocalizer NewLocalizer { get; set; }
		[Inject] private ILocalStorageService LocalStorage { get; set; }

		private const string LocalStorageModelKey = "New_UserData";


		protected override async Task OnInitializedAsync()
		{
			testTypes.Add(new TestType { Value = false, Title = NewLocalizer.Negative });
			testTypes.Add(new TestType { Value = true, Title = NewLocalizer.Positive });

#if DEBUG
			model.FullName = "Jan Novák";
			model.PernamentAddress = "Jiráskova 7, 739 11 Frýdlant nad Ostravicí";
			model.CurrentAddress = "Opletalova 1603/57, 110 00 Nové Město";
			model.DateOfBirth = new DateTime(1995, 10, 31);
			model.PhoneNumber = "+420 736 111 233";
			model.Email = "jan.novak@example.cz";
			model.PassportOrIdNumber = "123 457 887";
			model.TestDateTime = DateTime.Now;
			model.TestType = false;
			model.TestPlace = "Medical Testing s.r.o., AG CovidPoint, Želetavská ul., 140 00 Praha";
#endif

			// load from local storage, if some model has been already saved
			var savedModel = await LocalStorage.GetItemAsync<UserData>(LocalStorageModelKey);
			if (savedModel != null)
			{
				model = savedModel;
			}
		}

		protected async Task GenerateAsync()
		{
			// save to local storage
			await LocalStorage.SetItemAsync(LocalStorageModelKey, model);

			var data = await PdfGenerator.GetPdfAsync(model);

			await JsRuntime.InvokeVoidAsync("jsSaveAsFile", $"Test-{model.TestDateTime.Value:yyyyMMdd}.pdf", Convert.ToBase64String(data));
		}

		internal class TestType
		{
			public bool Value { get; set; }
			public string Title { get; set; }
		}
	}
}
