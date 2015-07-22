using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.EmailTemplate
{
    public class EmailTemplateIndexModel : BaseModel {
        public EmailTemplateIndexModel() {
            Details = new List<EmailTemplateModel>();
        }

        public List<EmailTemplateModel> Details { get; set; }
    }
    public class EmailTemplateModel : BaseModel
    {
        public int EmailTemplateId { get; set; }
        public string EmailTemplateName { get; set; }

        public string EmailTemplateBody { get; set; }
        public string EmailTemplateSubject { get; set; }
    }
}