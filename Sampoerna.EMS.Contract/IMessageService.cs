﻿using System.Collections.Generic;

namespace Sampoerna.EMS.Contract
{
    public interface IMessageService
    {
        bool SendEmailToList(List<string> to, string subject, string body, bool throwError = false);

        void SendEmail(string to, string subject, string body, bool throwError = false);

        //Adding List CC
        bool SendEmailToListWithCC(List<string> to, List<string> cc, string subject, string body, bool throwError = false);
    }
}