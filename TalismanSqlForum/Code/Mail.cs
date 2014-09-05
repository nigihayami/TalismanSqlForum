using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Configuration;

namespace TalismanSqlForum.Code
{
    public class Mail
    {
        public static void SendEmail(MailMessage mail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            var acc = ConfigurationManager.AppSettings["email_account"];
            var pass = ConfigurationManager.AppSettings["email_pass"];
            smtpServer.Credentials = new System.Net.NetworkCredential(acc, pass);
            smtpServer.Port = 587; // Gmail works on this port
            smtpServer.EnableSsl = true;

            mail.From = new MailAddress("noreply@talisman-sql.ru");
            try
            {
                smtpServer.Send(mail);
            }
            catch { };
            smtpServer.Dispose();
        }
    }
}