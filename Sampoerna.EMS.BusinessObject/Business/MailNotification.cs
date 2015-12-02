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
            CC = new List<string>();
            IsCCExist = false;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }

        public List<string> CC { get; set; }
        public bool IsCCExist { get; set; }
    }
}
