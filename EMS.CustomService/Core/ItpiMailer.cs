using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Sampoerna.EMS.CustomService.Core
{
    public class ItpiMailer
    {
        private static ItpiMailer instance = new ItpiMailer();
        private ItpiMailer()
        {
        }

        private SmtpClient ConfigSmtp()
        {
            try
            {
                var smtp = new SmtpClient()
                {
                    Host = ConfigurationManager.AppSettings["smtp_host"], // smtp server address here…
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]),
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["smtp_enable_ssl"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtp_username"], ConfigurationManager.AppSettings["smtp_password"]),
                    Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["smtp_timeout"]),
                };

                return smtp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MailMessage ConfigureEmail(string[] addresses, string[] ccs, string[] bccs, string[] attachments, string subject, string mailContent, bool htmlMail, string from = null, string displayName = null)
        {
            MailMessage email = new MailMessage();
            email.Subject = subject;
            email.Body = mailContent;
            email.IsBodyHtml = htmlMail;
            var sender = (string.IsNullOrEmpty(from)) ? ConfigurationManager.AppSettings["smtp_email"] : from;
            var display = (string.IsNullOrEmpty(displayName)) ? ConfigurationManager.AppSettings["smtp_display_name"] : displayName;
            email.ReplyToList.Add(sender);
            email.From = new MailAddress(sender, displayName);
            foreach (string s in addresses)
            {
                email.To.Add(s);
            }

            if (attachments != null)
            {
                foreach (string s in attachments)
                {
                    Attachment item = null;
                    if (s.Contains("http://") || s.Contains("https://")) // url
                    {
                        var stream = new System.Net.WebClient().OpenRead(s);
                        item = new Attachment(stream, s.Substring(s.LastIndexOf("/")));
                    }
                    else // Local path
                    {
                        //var path = s;
                        //if (s.Contains("/")) // relative url
                        //{
                        //    path = HttpContent.Current.Server.MapPath("~/" + s);
                        //}
                        //item = new Attachment(path);
                    }
                    email.Attachments.Add(item);
                }
            }

            if (ccs != null)
            {
                foreach (string s in ccs)
                {
                    email.CC.Add(s);
                }
            }

            if (bccs != null)
            {
                foreach (string s in bccs)
                {
                    email.Bcc.Add(s);
                }
            }
            return email;
        }

        public bool SendEmail(string[] addresses, string[] ccs, string[] bccs, string[] attachments, string subject, string mailContent, bool htmlMail, string from = null, string displayName = null)
        {
            bool success = false;
            SmtpClient mailer = null;
            var root = ConfigurationManager.AppSettings["EmailLogPath"];
            if (!System.IO.Directory.Exists(root))
            {
                System.IO.Directory.CreateDirectory(root);
            }
            try
            {
                if (ConfigurationManager.AppSettings["UsingDefaultEmail"] == "true")
                {
                    mailer = new SmtpClient();
                   
                }
                else
                {
                    mailer = ConfigSmtp();
                }
                // Bypass email sender
                from = ConfigurationManager.AppSettings["DefaultSender"];
                displayName = ConfigurationManager.AppSettings["DefaultDisplay"];
                MailMessage message = ConfigureEmail(addresses, ccs, bccs, attachments, subject, mailContent, htmlMail, from, displayName);
                
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), "Log Started at " + DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss") + Environment.NewLine);
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), "Sending email using default email config: " + ConfigurationManager.AppSettings["UsingDefaultEmail"] + Environment.NewLine);

                mailer.Send(message);
                success = true;
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), "Completed email send successfully" + Environment.NewLine);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), "Email send failed. Exception: " + Environment.NewLine);
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), String.Format("Message: {0}", ex.Message) + Environment.NewLine);
                System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), String.Format("Stack Trace: {0}", ex.StackTrace) + Environment.NewLine);
                if (ex.InnerException != null)
                {
                    System.IO.File.AppendAllText(root + String.Format("Mailer Log {0}.txt", DateTime.Now.ToString("dd MMM yyyy")), String.Format("Inner Exception: {0}", ex.InnerException.Message) + Environment.NewLine);
                }
                
                //throw ex;

            }
            finally
            {
                mailer.Dispose();
            }
            return success;

        }



        public static ItpiMailer Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
