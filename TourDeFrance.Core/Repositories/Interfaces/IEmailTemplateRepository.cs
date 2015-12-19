using System;
using System.Collections.Generic;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IEmailTemplateRepository
	{
		DbMailTemplate CreateEmailTemplate(string name, Guid emailTemplateId);
		
		DbMailTemplate RestoreDefaultTemplate(Guid emailTemplateId);

		DbMailTemplate UpdateEmailTemplate(Guid emailTemplateId, Guid globalMailTemplateId, string name, string subjectTemplate,
										 string titleTemplate, string subTitleTemplate, string bodyTemplate,
										 string footerTemplate);

		DbMailTemplate DeleteMailTemplate(Guid emailTemplateId);

		IEnumerable<DbMailTemplate> GetAllMailTemplates();
		
		DbMailTemplate GetMailTemplateById(Guid id, bool throwIfNotExist = true);

		string GetBodyContent<T>(string mailTemplateType, Guid? templateId, T model);

		string GetSubjectContent<T>(string mailTemplateType, Guid? templateId, T model);

		string GetSampleBodyContent(Guid emailTemplateId);

		string GetSampleSubjectContent(Guid emailTemplateId);

		IEnumerable<DbGlobalMailTemplate> GetAllGlobalMailTemplates();

		DbGlobalMailTemplate GetGlobalMailTemplateById(Guid id, bool throwIfNotExist = true);
	}
}
