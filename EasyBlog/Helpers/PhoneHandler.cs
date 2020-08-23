using System;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EasyBlog.Helpers
{
    public class PhoneHandler
    {
        private string accountSid = ConfigurationManager.AppSettings["AccountSid"];
        private string authToken = ConfigurationManager.AppSettings["AuthToken"];
        private string phone = ConfigurationManager.AppSettings["Phone"];

        public string SendSms(string code, string to)
        {
            try
            {
                TwilioClient.Init(this.accountSid, this.authToken);
                var message = MessageResource.Create(
                    body: "Verification code: " + code,
                    from: new Twilio.Types.PhoneNumber(this.phone),
                    to: new Twilio.Types.PhoneNumber(to)
                );
                return message.Sid;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ResponseMessages.SMSException;
            }
        }
    }
}