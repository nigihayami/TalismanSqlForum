using System.Net.Mail;
using System.Configuration;

namespace TalismanSqlForum.Code
{
    public static class Mail
    {
        public static void SendEmail(MailMessage mail)
        {
            var acc = ConfigurationManager.AppSettings["email_account"];
            var pass = ConfigurationManager.AppSettings["email_pass"];
            var smtpServer = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new System.Net.NetworkCredential(acc, pass),
                Port = 587,
                EnableSsl = true
            };

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