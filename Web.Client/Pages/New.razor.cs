﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTG.CovidTestsGenerator.Contracts.Model;
using CTG.CovidTestsGenerator.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CTG.CovidTestsGenerator.Web.Client.Resources.Pages;

namespace CTG.CovidTestsGenerator.Web.Client.Pages
{
	public partial class New
	{
		private readonly UserData model = new();

		private readonly List<TestType> testTypes = new();

		[Inject] public IJSRuntime JsRuntime { get; set; }
		[Inject] public IPdfGenerator PdfGenerator { get; set; }
		[Inject] protected INewLocalizer NewLocalizer { get; set; }
		[Inject] protected CTG.CovidTestsGenerator.Resources.Model.IUserDataLocalizer UserDataLocalizer { get; set; }

		protected override void OnInitialized()
		{
			base.OnInitialized();

			testTypes.Add(new TestType { Value = false, Title = NewLocalizer.Negative });
			testTypes.Add(new TestType { Value = true, Title = NewLocalizer.Positive });

			model.FullName = UserDataLocalizer.DefaultFullName;
			model.PermanentAddress = UserDataLocalizer.DefaultPermanentAddress;
			model.CurrentAddress = UserDataLocalizer.DefaultCurrentAddress;
			model.Email = UserDataLocalizer.DefaultEmail;
			model.TestPlace = UserDataLocalizer.DefaultTestPlace;
		}

		protected async Task GenerateAsync()
		{
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
