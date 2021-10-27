using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using CTG.CovidTestsGenerator.Web.Client.Resources.Pages;

namespace CTG.CovidTestsGenerator.Web.Client.Pages
{
	public partial class HomeIndex
	{
		[Inject] protected IHomeIndexLocalizer HomeIndexLocalizer { get; set; }
	}
}
