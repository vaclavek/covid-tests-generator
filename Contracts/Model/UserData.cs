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

		public DateTime? DateOfBirth { get; set; } = new DateTime(1995, 10, 31);
		public string PhoneNumber { get; set; }

		public string Email { get; set; }
		public string PassportOrIdNumber { get; set; } = "123 457 887";

		public DateTime TestDateTime
		{
			get
			{
				Random r = new Random();
				return new DateTime(TestDate.Year, TestDate.Month, TestDate.Day, TestHour, TestMinute, r.Next(0, 59));
			}
		}
		public DateTime TestDate { get; set; } = DateTime.Now;
		public int TestHour { get; set; } = 15;
		public int TestMinute { get; set; } = 40;

		public bool TestType { get; set; } = false;
		public string TestPlace { get; set; }

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
