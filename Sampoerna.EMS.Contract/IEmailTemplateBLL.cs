using Sampoerna.EMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.Contract
{
   public interface IEmailTemplateBLL
    {
       List<EMAIL_TEMPLATE> getAllEmailTemplates();

       EMAIL_TEMPLATE getEmailTemplateById(long id);

       void Save(EMAIL_TEMPLATE record);

       

    }
}
