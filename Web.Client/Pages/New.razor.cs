using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CTG.CovidTestsGenerator.Contracts;
using CTG.CovidTestsGenerator.Contracts.Model;
using CTG.CovidTestsGenerator.Resources;
using CTG.CovidTestsGenerator.Web.Client.Resources.Pages;
using CTG.CovidTestsGenerator.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace CTG.CovidTestsGenerator.Web.Client.Pages
{
	public partial class New
	{
		private UserData model = new();

		private readonly List<TestType> testTypes = new();
		private readonly List<TimeType> hourTypes = new();
		private readonly List<TimeType> MinuteTypes = new();

		[Inject] public IJSRuntime JsRuntime { get; set; }
		[Inject] public IPdfGenerator PdfGenerator { get; set; }
		[Inject] protected INewLocalizer NewLocalizer { get; set; }
		[Inject] protected IStringLocalizer<Global> GlobalLocalizer { get; set; }
		[Inject] private ILocalStorageService LocalStorage { get; set; }
		[Inject] protected CTG.CovidTestsGenerator.Resources.Model.IUserDataLocalizer UserDataLocalizer { get; set; }

		private const string LocalStorageModelKey = "New_UserData";

		private ICollection<UserData> SavedProfiles { get; set; }
		private string SavedProfile
		{
			get
			{
				return _savedProfile;
			}
			set
			{
				_savedProfile = value;
				EmployeeSelectionChanged(value);
			}
		}
		private string _savedProfile;

		private string NullText => GlobalLocalizer["SelectNull"].Value;

		protected override async Task OnInitializedAsync()
		{
			testTypes.Add(new TestType { Value = false, Title = NewLocalizer.Negative });
			testTypes.Add(new TestType { Value = true, Title = NewLocalizer.Positive });

			for (int hour = 0; hour <= 23; hour++)
			{
				hourTypes.Add(new TimeType { Value = hour, Title = hour.ToString("D2") });
			}
			for (int minute = 0; minute <= 59; minute += 5)
			{
				MinuteTypes.Add(new TimeType { Value = minute, Title = minute.ToString("D2") });
			}

			model.FullName = UserDataLocalizer.DefaultFullName;
			model.PermanentAddress = UserDataLocalizer.DefaultPermanentAddress;
			model.CurrentAddress = UserDataLocalizer.DefaultCurrentAddress;
			model.Email = UserDataLocalizer.DefaultEmail;
			model.TestPlace = UserDataLocalizer.DefaultTestPlace;
			model.PhoneNumber = UserDataLocalizer.DefaultPhoneNumber;
		}

		protected async Task GenerateAsync()
		{
			if (!SavedProfiles.Any(x => x.SavedUserProfileKey == model.SavedUserProfileKey))
			{
				SavedProfiles.Add(model);
			}
			await SaveUserProfilesAsync();

			var data = await PdfGenerator.GetPdfAsync(model);

			await JsRuntime.InvokeVoidAsync("jsSaveAsFile", $"Test-{model.TestDate:yyyyMMdd}.pdf", Convert.ToBase64String(data));
		}

		internal class TestType
		{
			public bool Value { get; set; }
			public string Title { get; set; }
		}

		internal class TimeType
		{
			public int Value { get; set; }
			public string Title { get; set; }
		}
	}
}
