using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DP.Notifications.Services
{
    public class EmailSenderService
    {
        public void SendMessage(string emailAddress, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("code.flow.mail@gmail.com", "Fcu4e8qdxG7xCF6s"),
                EnableSsl = true,
            };
            smtpClient.Send("code.flow.mail@gmail.com", emailAddress, subject, body);
        }
    }
}
