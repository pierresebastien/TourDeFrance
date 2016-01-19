using System;
using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	// TODO: to use for default mail tempaltes too => improve repository
	[Alias("mail_templates")]
	public class DbMailTemplate : BaseOwnObjectNameable
	{
		public const string LostPassword = "LostPassword";
		public const string UserCreated = "UserCreated";

		[ForeignKey(typeof(DbGlobalMailTemplate))]
		[Alias("global_mail_template_id")]
		public Guid GlobalTemplateId { get; set; }

		[ForeignKey(typeof(DbMailTemplate))]
		[Alias("default_mail_template_id")]
		public Guid? DefaultMailTemplateId { get; set; }

		[Alias("type")]
		public string Type { get; set; }

		[Alias("subject_template")]
		public string SubjectTemplate { get; set; }

		[Alias("title_template")]
		public string TitleTemplate { get; set; }

		[Alias("sub_title_template")]
		public string SubTitleTemplate { get; set; }

		[Alias("body_template")]
		public string BodyTemplate { get; set; }

		[Alias("footer_template")]
		public string FooterTemplate { get; set; }

		[Alias("number_of_override")]
		public int? NumberOfOverride { get; set; }
	}
}
