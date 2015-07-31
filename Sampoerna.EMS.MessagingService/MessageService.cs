using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Sampoerna.EMS.Contract;
using Sampoerna.EMS.Core;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.MessagingService
{
    public class MessageService : IMessageService
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private ILogger _logger;

        public MessageService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="throwError"></param>
        public void SendEmailToList(List<string> to, string subject, string body, bool throwError = false)
        {
            try
            {
                var actualTo = new List<string>();
               
                actualTo.AddRange(to);
               

                var smtpClient = new SmtpClient();
              

                var mailMessage = new MailMessage {IsBodyHtml = true};
                actualTo.ForEach(s => mailMessage.To.Add(s.Trim()));
               
                mailMessage.Body = body;
             
                mailMessage.Subject = subject;
                //Make sure the client and the message are disposed when the asynch send is done
                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();
                    mailMessage.Dispose();
                };

                _logger.Debug(string.Format("Messager.MailSent from : {0}; to : {1}; subject {2}; body : {3}",
                    mailMessage.From.Address, string.Join(",", mailMessage.To.Select(a => a.Address)), mailMessage.Subject, mailMessage.Body));
                smtpClient.Send(mailMessage);
                //smtpClient.SendAsync(mailMessage, null); //Sendasynch doesn't have the time to send in some case, no way to make sure it waits 'till the mail is sent for now.
            }
            catch (Exception ex)
            {
                // error not thrown, log error here
                _logger.Error("MessagingService.Messager SendEmail: " + ex);

                //throw the error only if needed for upper layers logic
                if (throwError)
                    throw;
            }
        }

        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="throwError"></param>
        public void SendEmail(string to, string subject, string body, bool throwError = false)
        {
            var actualTo = new List<string> { to };
            SendEmailToList(actualTo, subject, body, throwError);
        }
    }
}
