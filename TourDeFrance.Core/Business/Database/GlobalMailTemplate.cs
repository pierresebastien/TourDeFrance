using System;
using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("global_mail_templates")]
	public class DbGlobalMailTemplate : BaseNameable
	{
		public static readonly Guid DefaultId = new Guid("{10000000-0000-0000-0000-000000000001}");

		[Alias("template")]
		public string Template { get; set; }
	}
}
