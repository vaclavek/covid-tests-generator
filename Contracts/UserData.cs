using System;
using System.ComponentModel.DataAnnotations;

namespace CTG.CovidTestsGenerator.Contracts
{
	public class UserData
	{
		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public string FullName { get; set; }

		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public string PernamentAddress { get; set; }

		public string CurrentAddress { get; set; }

		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public DateTime? DateOfBirth { get; set; }

		public string PhoneNumber { get; set; }

		public string Email { get; set; }

		public string PassportOrIdNumber { get; set; }

		[Required(ErrorMessage = "{0} je vyžadováno")]
		public DateTime? TestDateTime { get; set; }

		public bool TestType { get; set; }

		public string TestPlace { get; set; }

		public string SavedUserProfileTitle => $"{FullName} ({DateOfBirth.Value:dd.MM.yyyy}), {PernamentAddress}";
		public string SavedUserProfileKey => $"{FullName}_{DateOfBirth.Value:dd.MM.yyyy}_{PernamentAddress}";
	}
}
