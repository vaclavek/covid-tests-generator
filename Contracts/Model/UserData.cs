using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using CTG.CovidTestsGenerator.Resources.Model;
using Microsoft.AspNetCore.Components;

namespace CTG.CovidTestsGenerator.Contracts.Model
{
	public class UserData
	{
		public string FullName { get; set; }
		public string PermanentAddress { get; set; }
		public string CurrentAddress { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string PassportOrIdNumber { get; set; }
		public bool TestType { get; set; }
		public string TestPlace { get; set; }

		public DateTime TestDateTime => new DateTime(TestDate.Year, TestDate.Month, TestDate.Day, TestHour, TestMinute, new Random().Next(0, 59));
		public DateTime TestDate { get; set; }
		public int TestHour { get; set; }
		public int TestMinute { get; set; }

		public string SavedUserProfileTitle => $"{FullName} ({DateOfBirth.Value:dd.MM.yyyy}), {PermanentAddress}";
		public string SavedUserProfileKey => $"{FullName}_{DateOfBirth.Value:dd.MM.yyyy}_{PermanentAddress}";

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public class UserDataDtoValidator : AbstractValidator<UserData>
		{
			public UserDataDtoValidator(IUserDataLocalizer userDataLocalizer)
			{
				RuleFor(c => c.FullName)
					.NotEmpty()
					.MaximumLength(100)
					.WithName(userDataLocalizer.FirstAndSecondName);

				RuleFor(c => c.PermanentAddress)
					.NotEmpty()
					.MaximumLength(100)
					.WithName(userDataLocalizer.PermanentAddress);

				RuleFor(c => c.DateOfBirth)
					.NotEmpty()
					.WithName(userDataLocalizer.DateOfBirth);

				RuleFor(c => c.TestDate)
					.NotEmpty()
					.WithName(userDataLocalizer.TestDateTime);
			}
		}
	}
}
