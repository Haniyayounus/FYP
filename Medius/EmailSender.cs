using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Medius
{
    public class EmailSender
    {
        private readonly IConfiguration _config;
        public async Task SendEmail(string htmlContent, string subject, string toAddress)
        {
            string key = _config.GetValue<string>("SendGrid:SendGridKey");
            string email = _config.GetValue<string>("SendGrid:SendGridUser");
            string name = _config.GetValue<string>("SendGrid:younushaniya@gmail.com");

            var client = new SendGridClient(key);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(email, name),
                Subject = subject,
                HtmlContent = htmlContent
            };

            msg.AddTo(new EmailAddress(toAddress));

            await client.SendEmailAsync(msg);
        }

    }
}