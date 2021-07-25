using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Utility
{
    //public class EmailSender : IEmailSender
    //{
    //    private readonly EmailOptions emailOptions;
    //    private readonly IConfiguration _config;

    //    public EmailSender(IOptions<EmailOptions> options, IConfiguration config) 
    //    {
    //        emailOptions = options.Value;
    //        _config = config;
    //    }
    //    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    //    {
    //        return Execute(emailOptions.SendGridKey, subject, htmlMessage, email);
    //    }
    //    private Task Execute(string sendGridKEy, string subject, string message, string email)
    //    {
    //        sendGridKEy = _config.GetValue<string>("SendGrid:SendGridKey");
    //        var client = new SendGridClient(sendGridKEy);
    //        var msg = new SendGridMessage
    //        {
    //            From = new EmailAddress("younushaniya@google.com", "Team Medius"),
    //            Subject = subject,
    //            HtmlContent = message
    //        }; 
    //        return client.SendEmailAsync(msg);
    //    }
    //}
    public class EmailSender
    {
        public string SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = new MailAddress("mediusdigitalaccess@gmail.com");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "mediusapp544";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return "message sent";
        }
        
           
        //public Task SendAsync(IdentityMessage message)
        //{
        //    const string sentFrom = "younushaniya@google.com";

        //    MailMessage mail = new MailMessage { From = new MailAddress(sentFrom, "Team Medius") };
        //    mail.To.Add(message.Destination);

        //    mail.Subject = message.Subject;
        //    mail.Body = message.Body;
        //    mail.IsBodyHtml = true;
        //    SmtpClient smtp = new SmtpClient()
        //    {
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        Timeout = 600000,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(sentFrom, ")4(qx^n;sbZf8%S!"),
        //    };

        //    return smtp.SendMailAsync(mail);
        //}
    }
}
