using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IMessageService
    {
        void SendEmailToList(string from, List<string> to, string subject, string body, bool throwError = false);

        void SendEmail(string from, string to, string subject, string body, bool throwError = false); 
    }
}