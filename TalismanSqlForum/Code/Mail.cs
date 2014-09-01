using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace TalismanSqlForum.Code
{
    public class Mail
    {
        public static void SendEmail(MailMessage mail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Credentials = new System.Net.NetworkCredential("forumtalismanwebru", "ckj;ysqgfhjkm");
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