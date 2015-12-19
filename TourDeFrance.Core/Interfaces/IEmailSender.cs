using MimeKit;
using System;
using System.Collections.Generic;
namespace TourDeFrance.Core.Interfaces
{
	public interface IEmailSender
	{
		void SendEmail<T>(IEnumerable<string> toRecipients, string emailType, Guid? templateId, T model,
			IEnumerable<string> ccRecipients = null, IEnumerable<MimePart> attachments = null);
	}
}
