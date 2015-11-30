using Sampoerna.EMS.BusinessObject;
using System.Collections.Generic;
using Sampoerna.EMS.BusinessObject.DTOs;
using Sampoerna.EMS.BusinessObject.Inputs;

namespace Sampoerna.EMS.Contract
{
   public interface IEmailTemplateBLL
    {
       List<EMAIL_TEMPLATE> getAllEmailTemplates();

       EMAIL_TEMPLATE getEmailTemplateById(long id);

       void Save(EMAIL_TEMPLATE record);

       EMAIL_TEMPLATEDto GetByDocumentAndActionType(EmailTemplateGetByDocumentAndActionTypeInput input);

    }
}
