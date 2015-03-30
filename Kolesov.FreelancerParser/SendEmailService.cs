using Kolesov.Domain.Interfaces;
using Kolesov.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.FreelancerParser
{
    public class SendEmailService : INotificationService
    {
        public void SendNotification(User sendToUser, string message)
        {
            var fromAddress = new MailAddress("kolesov.statistics@gmail.com", "Statistics");
            var toAddress = new MailAddress(sendToUser.Email, sendToUser.Name);
            var fromPassword = "kolesov.password";
            var subject = "New project";
            var body = message;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var email = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(email);
            }
        }
    }
}
