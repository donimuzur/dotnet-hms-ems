using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IMessageService
    {
        void SendEmailToList(List<string> to, string subject, string body, bool throwError = false);

        void SendEmail(string to, string subject, string body, bool throwError = false);

        //Adding List CC
        void SendEmailToListWithCC(List<string> to, List<string> cc, string subject, string body, bool throwError = false);
    }
}