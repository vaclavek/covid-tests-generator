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
		public string PhoneNumber { get; set; } = "+420 777 147 977";

		public string Email { get; set; }
		public string PassportOrIdNumber { get; set; } = "123 457 887";

		public DateTime? TestDateTime { get; set; } = DateTime.Now;

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

				RuleFor(c => c.TestDateTime)
					.NotEmpty()
					.WithName(userDataLocalizer.TestDateTime);
			}
		}
	}
}
