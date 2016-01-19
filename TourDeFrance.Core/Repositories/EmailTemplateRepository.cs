using DotLiquid;
using DotLiquid.NamingConventions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SimpleStack.Orm;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Business.Email;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class EmailTemplateRepository : BaseRepository, IEmailTemplateRepository
	{
		protected const string MailTemplateObjectName = "Mail template";
		protected const string GlobalMailTemplateObjectName = "Global mail template";
		protected readonly IDictionary<string, DbMailTemplate> DefaultMailTemplates;

		public EmailTemplateRepository(IDialectProvider dialectProvider, ApplicationConfig config)
		{
			Template.RegisterSafeType(typeof(Exception), new[] { "Message", "StackTrace" });
			Template.RegisterSafeType(typeof(Config), new[] { "PublicUri", "ApplicationName" });
			Template.RegisterSafeType(typeof(AuthenticatedUser), new[] { "UserName", "DisplayName", "Email" });
			// TODO:check if usage is needed
			// Template.RegisterFilter(typeof(CustomLiquidFilters));
			Template.NamingConvention = new CSharpNamingConvention();

			using (var scope = new TransactionScope(dialectProvider, config))
			{
				DefaultMailTemplates =
					scope.Connection.Select<DbMailTemplate>(x => x.DefaultMailTemplateId == null).ToDictionary(x => x.Type, x => x);
				scope.Complete();
			}
		}

		public DbMailTemplate CreateEmailTemplate(string name, Guid emailTemplateId)
		{
			EnsureUserIsAdmin();
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				EnsureObjectWithSameNameDoesNotExist<DbDrink>(name, MailTemplateObjectName);
				DbMailTemplate baseMailTemplate = GetMailTemplateById(emailTemplateId);
				DbMailTemplate defaultMailTemplate = DefaultMailTemplates[baseMailTemplate.Type];

				if (defaultMailTemplate.NumberOfOverride.HasValue)
				{
					if (scope.Connection.Count<DbMailTemplate>(x => x.DefaultMailTemplateId == defaultMailTemplate.Id) >=
					    defaultMailTemplate.NumberOfOverride.Value)
					{
						throw new TourDeFranceException(
							$"Can't have more than {defaultMailTemplate.NumberOfOverride.Value} template for '{defaultMailTemplate.Name}'");
					}
				}

				var template = new DbMailTemplate
				               {
					               Name = name,
					               SubjectTemplate = baseMailTemplate.SubjectTemplate,
					               TitleTemplate = baseMailTemplate.TitleTemplate,
					               SubTitleTemplate = baseMailTemplate.SubTitleTemplate,
					               BodyTemplate = baseMailTemplate.BodyTemplate,
					               FooterTemplate = baseMailTemplate.FooterTemplate,
					               GlobalTemplateId = baseMailTemplate.GlobalTemplateId,
					               DefaultMailTemplateId = defaultMailTemplate.Id,
					               NumberOfOverride = defaultMailTemplate.NumberOfOverride,
					               Type = defaultMailTemplate.Type
				               };
				template.BeforeInsert();
				template.SetOwner();
				scope.Connection.Insert(template);
				scope.Complete();

				return template;
			}
		}

		public DbMailTemplate RestoreDefaultTemplate(Guid emailTemplateId)
		{
			EnsureUserIsAdmin();

			using (var scope = new TransactionScope())
			{
				DbMailTemplate template = GetMailTemplateById(emailTemplateId);
				if (!template.DefaultMailTemplateId.HasValue)
				{
					throw new TourDeFranceException("Cannot edit default mail template");
				}
				EnsureUserHasRightToManipulateObject(template, ActionType.Update, MailTemplateObjectName);

				DbMailTemplate defaultTemplate = DefaultMailTemplates[template.Type];
				template.SubjectTemplate = defaultTemplate.SubjectTemplate;
				template.TitleTemplate = defaultTemplate.TitleTemplate;
				template.SubTitleTemplate = defaultTemplate.SubTitleTemplate;
				template.BodyTemplate = defaultTemplate.BodyTemplate;
				template.FooterTemplate = defaultTemplate.FooterTemplate;
				template.GlobalTemplateId = defaultTemplate.GlobalTemplateId;
				template.BeforeUpdate();
				scope.Connection.Update<DbMailTemplate>(template);
				scope.Complete();
				return template;
			}
		}

		// TODO: add checks on arguments?
		public DbMailTemplate UpdateEmailTemplate(Guid emailTemplateId, Guid globalMailTemplateId, string name,
			string subjectTemplate, string titleTemplate, string subTitleTemplate,
			string bodyTemplate, string footerTemplate)
		{
			EnsureUserIsAdmin();
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				DbMailTemplate template = GetMailTemplateById(emailTemplateId);
				if (!template.DefaultMailTemplateId.HasValue)
				{
					throw new TourDeFranceException("Cannot edit default mail template");
				}
				EnsureUserHasRightToManipulateObject(template, ActionType.Update, MailTemplateObjectName);
				EnsureObjectWithSameNameDoesNotExist(name, MailTemplateObjectName, template);

				template.SubjectTemplate = subjectTemplate;
				template.TitleTemplate = titleTemplate;
				template.SubTitleTemplate = subTitleTemplate;
				template.BodyTemplate = bodyTemplate;
				template.FooterTemplate = footerTemplate;
				template.GlobalTemplateId = globalMailTemplateId;
				template.Name = name;
				template.BeforeUpdate();
				scope.Connection.Update<DbMailTemplate>(template);
				scope.Complete();
				return template;
			}
		}

		public DbMailTemplate DeleteMailTemplate(Guid emailTemplateId)
		{
			EnsureUserIsAdmin();
			using (var scope = new TransactionScope())
			{
				DbMailTemplate template = GetMailTemplateById(emailTemplateId);
				if (!template.DefaultMailTemplateId.HasValue)
				{
					throw new TourDeFranceException("Cannot delete default mail template");
				}
				EnsureUserHasRightToManipulateObject(template, ActionType.Delete, MailTemplateObjectName);
				scope.Connection.DeleteAll<DbMailTemplate>(x => x.Id == emailTemplateId);
				scope.Complete();
				return template;
			}
		}

		public IEnumerable<DbMailTemplate> GetAllMailTemplates()
		{
			return GetAllDbObjects<DbMailTemplate>();
		}

		public DbMailTemplate GetMailTemplateById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbMailTemplate>(id, MailTemplateObjectName, throwIfNotExist);
		}

		public string GetBodyContent<T>(string mailTemplateType, Guid? templateId, T model)
		{
			CultureInfo current = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			try
			{
				DbMailTemplate template = templateId.HasValue
					? GetMailTemplateById(templateId.Value)
					: DefaultMailTemplates[mailTemplateType];
				var m = new
				        {
					        Model = model,
					        Context.Current.Config,
					        Context.Current.User
				        };
				var globalTemplateModel =
					new 
					{
						Title = Template.Parse(template.TitleTemplate).Render(Hash.FromAnonymousObject(m)),
						SubTitle = Template.Parse(template.SubTitleTemplate).Render(Hash.FromAnonymousObject(m)),
						Body = Template.Parse(template.BodyTemplate).Render(Hash.FromAnonymousObject(m)),
						Footer = Template.Parse(template.FooterTemplate).Render(Hash.FromAnonymousObject(m))
					};

				Template globalTemplate = Template.Parse(GetGlobalMailTemplateById(template.GlobalTemplateId).Template);
				return globalTemplate.Render(Hash.FromAnonymousObject(globalTemplateModel));
			}
			catch (Exception exception)
			{
				return exception.Message;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = current;
			}
		}

		public string GetSubjectContent<T>(string mailTemplateType, Guid? templateId, T model)
		{
			CultureInfo current = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			try
			{
				DbMailTemplate template = templateId.HasValue
					? GetMailTemplateById(templateId.Value)
					: DefaultMailTemplates[mailTemplateType];
				var m = new
				        {
					        Model = model,
					        Context.Current.Config,
					        Context.Current.User
				        };
				return Template.Parse(template.SubjectTemplate)
					.Render(Hash.FromAnonymousObject(m))
					.Replace('\r', ' ')
					.Replace('\n', ' ');
			}
			catch (Exception e)
			{
				return e.Message;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = current;
			}
		}

		public string GetSampleBodyContent(Guid emailTemplateId)
		{
			DbMailTemplate template = GetMailTemplateById(emailTemplateId);
			return GetBodyContent(template.Type, template.Id, GetSampleModel(template.Type));
		}

		public string GetSampleSubjectContent(Guid emailTemplateId)
		{
			DbMailTemplate template = GetMailTemplateById(emailTemplateId);
			return GetSubjectContent(template.Type, template.Id, GetSampleModel(template.Type));
		}

		public IEnumerable<DbGlobalMailTemplate> GetAllGlobalMailTemplates()
		{
			return GetAllDbObjects<DbGlobalMailTemplate>();
		}

		public DbGlobalMailTemplate GetGlobalMailTemplateById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbGlobalMailTemplate>(id, GlobalMailTemplateObjectName, throwIfNotExist);
		}

		protected virtual BaseEmailModel GetSampleModel(string mailTemplateType)
		{
			BaseEmailModel result;
			switch (mailTemplateType)
			{
				case DbMailTemplate.LostPassword:
					result = new LostPasswordEmailModel
					{
						AuthToken = "AUTHTOKEN",
						LoginName = Context.Current.User.Username,
						UserName = Context.Current.User.DisplayName,
						ExpirationDate = DateTime.Now.AddHours(Context.Current.Config.NumberOfHourBeforeTokenExpiration)
					};
					break;
				case DbMailTemplate.UserCreated:
					result = new UserCreatedEmailModel { Password = "Xs34HJ-21", DisplayName = "John Doe", UserName = "8847758" };
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return result;
		}
	}
}
