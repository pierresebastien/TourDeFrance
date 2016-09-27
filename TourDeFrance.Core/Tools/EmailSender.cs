using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Configuration;
using System.Net.Mail;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Repositories.Interfaces;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace TourDeFrance.Core.Tools
{
	public class EmailSender : IEmailSender
	{
		private readonly IEmailTemplateRepository _emailTemplateRepository;
		private readonly Config _config;
		private static readonly ILog Logger = LogProvider.For<EmailSender>();

		protected static readonly Action<MimeMessage> SendMail;
		protected static readonly SmtpSection SmtpConfig;

		static EmailSender()
		{
			SmtpConfig = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
			if (SmtpConfig != null)
			{
				switch (SmtpConfig.DeliveryMethod)
				{
					case SmtpDeliveryMethod.Network:
						if (SmtpConfig.Network == null)
						{
							SendMail = x => { throw new ConfigurationErrorsException("Smtp network section is not defined in config file"); };
						}
						else
						{
							if (string.IsNullOrWhiteSpace(SmtpConfig.Network.Host))
							{
								SendMail = x => { throw new ConfigurationErrorsException("Smtp host is not defined in config file"); };
							}
							else
							{
								SendMail = x => {
									using (SmtpClient client = new SmtpClient())
									{
										client.Connect(SmtpConfig.Network.Host, SmtpConfig.Network.Port);
										if (!string.IsNullOrWhiteSpace(SmtpConfig.Network.UserName) &&
										   !string.IsNullOrWhiteSpace(SmtpConfig.Network.Password))
										{
											client.Authenticate(SmtpConfig.Network.UserName, SmtpConfig.Network.Password);
										}
										client.Send(x);
										client.Disconnect(true);
									}
								};
							}
						}
						break;
					case SmtpDeliveryMethod.SpecifiedPickupDirectory:
						if (SmtpConfig.SpecifiedPickupDirectory == null)
						{
							SendMail = x => { throw new ConfigurationErrorsException("Pickup directory is not defined in config file"); };
						}
						else
						{
							SendMail = x => {
								x.WriteTo(Path.Combine(SmtpConfig.SpecifiedPickupDirectory.PickupDirectoryLocation,
									Guid.NewGuid() + ".eml"));
							};
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				SendMail = x => { throw new ConfigurationErrorsException("Smtp section is not defined in config file"); };
			}
		}

		public EmailSender(IEmailTemplateRepository emailTemplateRepository, Config config)
		{
			_emailTemplateRepository = emailTemplateRepository;
			_config = config;
		}

		public virtual void SendEmail<T>(IEnumerable<string> toRecipients, string emailType, Guid? templateId, T model,
			IEnumerable<string> ccRecipients = null, IEnumerable<MimePart> attachments = null)
		{
			toRecipients.EnsureIsNotNull("To recipients cannot be null");

			ccRecipients = ccRecipients ?? new string[0];
			attachments = attachments ?? new MimePart[0];

			MimeMessage mail = new MimeMessage
			                   {
				                   Subject = _emailTemplateRepository.GetSubjectContent(emailType, templateId, model)
			                   };
			Logger.DebugFormat("Sending email : {0}", mail.Subject);
			mail.From.Add(new MailboxAddress(_config.SmtpSenderDisplayName, _config.SmtpSenderEmailAddress));
			Logger.DebugFormat("From : {0}", mail.From);
			foreach (string recipient in toRecipients)
			{
				Logger.DebugFormat("To: {0}", recipient);
				mail.To.Add(new MailboxAddress(recipient, recipient));
			}
			foreach (string recipient in ccRecipients)
			{
				Logger.DebugFormat("Cc: {0}", recipient);
				mail.Cc.Add(new MailboxAddress(recipient, recipient));
			}

			var multipart = new Multipart("mixed")
			                {
				                new TextPart("html")
				                {
					                Text = _emailTemplateRepository.GetBodyContent(emailType, templateId, model),
					                ContentTransferEncoding = ContentEncoding.QuotedPrintable
				                }
			                };
			foreach (MimePart attachment in attachments)
			{
				multipart.Add(attachment);
			}
			mail.Body = multipart;

			SendMail(mail);
		}
	}
}
