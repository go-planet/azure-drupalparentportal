using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Drupal.ParentPortal.Repository.Utilities
{
    public class EmailMessage
    {
        public List<Attachment> Attachments { get; set; }
        public string FromAddress { get; set; }
        public List<string> ToAddress { get; set; }
        public List<string> CcAddress { get; set; }
        public List<string> BccAddress { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SmtpAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SslEnabled { get; set; }

        public EmailMessage()
        {
            FromAddress = "";
            ToAddress = new List<string>();
            BccAddress = new List<string>();
            CcAddress = new List<string>();
            IsBodyHtml = true;
            Subject = "";
            Body = "";
            SmtpAddress = "mail.exadev.com";
            Port = 25;
            UserName = null;
            Password = null;
            SslEnabled = true;
            Attachments = new List<Attachment>();
        }


        
     
        public bool Send()
        {

            bool mailSent = false;

            try
            {
                MailMessage Mail = new MailMessage();
                if(ToAddress != null) ToAddress.ForEach(x => Mail.To.Add(x));
                if (CcAddress != null) CcAddress.ForEach(x => Mail.CC.Add(x));
                if (BccAddress != null) BccAddress.ForEach(x => Mail.Bcc.Add(x));
                Mail.From = new MailAddress(this.FromAddress);
                Mail.Subject = this.Subject;
                Mail.Body = this.Body;
                Mail.IsBodyHtml = this.IsBodyHtml;

                if (Attachments.Count > 0)
                    foreach (var attchment in Attachments)
                        Mail.Attachments.Add(attchment);
               
                SmtpClient SMTP = new SmtpClient(this.SmtpAddress);
                SMTP.UseDefaultCredentials = false;
                SMTP.Credentials = new System.Net.NetworkCredential(this.UserName, this.Password);
                
                SMTP.Port = this.Port;
                SMTP.Host = this.SmtpAddress;
                SMTP.EnableSsl = this.SslEnabled;
                SMTP.Send(Mail);  
                SMTP.Dispose();
                Mail.Dispose();
                mailSent = true;

            }
            catch (Exception ex)
            {
                mailSent = false;
            }

            return mailSent;

        }
   

    }
}
