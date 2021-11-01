using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using CTG.CovidTestsGenerator.Contracts;
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

		[Inject] public IJSRuntime JsRuntime { get; set; }
		[Inject] public IPdfGenerator PdfGenerator { get; set; }
		[Inject] protected INewLocalizer NewLocalizer { get; set; }
		[Inject] protected IStringLocalizer<Global> GlobalLocalizer { get; set; }
		[Inject] private ILocalStorageService LocalStorage { get; set; }

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

			await JsRuntime.InvokeVoidAsync("jsSaveAsFile", $"Test-{model.TestDateTime.Value:yyyyMMdd}.pdf", Convert.ToBase64String(data));
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
	}
}
