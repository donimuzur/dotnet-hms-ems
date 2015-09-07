using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.BusinessObject.Business
{
    public class MailNotification
    {
        public MailNotification()
        {
            To = new List<string>();
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
    }
}
