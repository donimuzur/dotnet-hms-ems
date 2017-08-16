using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class EmailContent
    {
        public EmailContent()
        {
            Variables = new List<EmailVariable>();
        }
        public long Id { set; get; }
        public string Name { set; get; }
        public string Content { set; get; }
        public string Subject { set; get; }
        public List<EmailVariable> Variables { set; get; }
    }
}