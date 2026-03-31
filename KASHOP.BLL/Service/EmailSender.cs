using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ta.damery2@gmail.com", "bpbd yqzg ukmo btif")
            };

            return client.SendMailAsync(
                new MailMessage(from: "ta.damery2@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {IsBodyHtml=true}
                );
        }
    }
}
