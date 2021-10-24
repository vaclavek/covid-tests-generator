using System;
using System.ComponentModel.DataAnnotations;

namespace CTG.CovidTestsGenerator.Contracts
{
	public class UserData
	{
		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public string FullName { get; set; } = "Ondřej Václavek";
		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public string PernamentAddress { get; set; } = "Pražská 123, 580 05 Brno nad Oslavicí";
		public string CurrentAddress { get; set; } = "Brněnská 456, 120 00 Praha 2 - Nové Město";

		[Required(ErrorMessage = "{0} je vyžadováno.", AllowEmptyStrings = false)]
		public DateTime? DateOfBirth { get; set; } = new DateTime(1995, 10, 31);
		public string PhoneNumber { get; set; } = "+420 777 147 977";

		public string Email { get; set; } = "toto.je.muj@email.cz";
		public string PassportOrIdNumber { get; set; } = "123 457 887";

		[Required(ErrorMessage = "{0} je vyžadováno")]
		public DateTime? TestDateTime { get; set; } = DateTime.Now;

		public bool TestType { get; set; } = false;
		public string TestPlace { get; set; } = "Medical Testing s.r.o., AG CovidPoint, Želetavská ul., 140 00 Praha";
	}
}
