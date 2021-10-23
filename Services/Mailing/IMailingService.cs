using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CTG.CovidTestsGenerator.Services.Mailing
{
	public interface IMailingService
	{
		void Send(MailMessage mailMessage);
	}
}
