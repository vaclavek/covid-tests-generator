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

#if DEBUG
			model.FullName = UserDataLocalizer.DefaultFullName;
			model.PermanentAddress = UserDataLocalizer.DefaultPermanentAddress;
			model.CurrentAddress = UserDataLocalizer.DefaultCurrentAddress;
			model.Email = UserDataLocalizer.DefaultEmail;
			model.TestPlace = UserDataLocalizer.DefaultTestPlace;

			model.DateOfBirth = new DateTime(1995, 10, 31);
			model.PhoneNumber = "+420 736 111 233";
			model.Email = "jan.novak@example.cz";
			model.PassportOrIdNumber = "123 457 887";
			model.TestDate = DateTime.Now.Date;
			model.TestHour = 18;
			model.TestMinute = 40;
			model.TestType = false;
			model.TestPlace = "Medical Testing s.r.o., AG CovidPoint, Želetavská ul., 140 00 Praha";
#endif

			await LoadUserProfilesAsync();
		}

		private void EmployeeSelectionChanged(string key)
		{
			var savedProfile = SavedProfiles.FirstOrDefault(x => x.SavedUserProfileKey == key);
			if (savedProfile != null)
			{
				model = savedProfile;
			}
		}

		protected async Task GenerateAsync()
		{
			if (!SavedProfiles.Any(x => x.SavedUserProfileKey == model.SavedUserProfileKey))
			{
				SavedProfiles.Add(model);
			}
			await SaveUserProfilesAsync();

			var data = await PdfGenerator.GetPdfAsync(model);

			await JsRuntime.InvokeVoidAsync("jsSaveAsFile", $"Test-{model.TestDateTime:yyyyMMdd}.pdf", Convert.ToBase64String(data));
		}

		private async Task LoadUserProfilesAsync()
		{
			// load from local storage, if some model has been already saved
			SavedProfiles = await LocalStorage.GetItemAsync<ICollection<UserData>>(LocalStorageModelKey) ?? new List<UserData>();
		}

		private async Task SaveUserProfilesAsync()
		{
			// save to local storage
			await LocalStorage.SetItemAsync(LocalStorageModelKey, SavedProfiles);
		}

		private async Task ClearAllLocalStoragesAsync()
		{
			await LocalStorage.RemoveItemAsync(LocalStorageModelKey);
			SavedProfile = null;
			await LoadUserProfilesAsync();
		}

		private async Task ClearSelectedLocalStorageAsync()
		{
			var profile = SavedProfiles.FirstOrDefault(x => x.SavedUserProfileKey == SavedProfile);
			if (profile != null)
			{
				SavedProfile = null;
				SavedProfiles.Remove(profile);
				await SaveUserProfilesAsync();
			}
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
