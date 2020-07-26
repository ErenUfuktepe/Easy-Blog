using EasyBlog.Models.RequestModels;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EasyBlog.Helpers
{
    public class EmailHandler
    {
        private string email = ConfigurationManager.AppSettings["SenderEmail"];
        private string password = ConfigurationManager.AppSettings["SenderPassword"];

        public bool SendEmail(EmailModel request)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.live.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(this.email, this.password);

                MailMessage message = new MailMessage(this.email, request.toEmail, request.subject, request.body);
                message.IsBodyHtml = true;
                message.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

    }
}