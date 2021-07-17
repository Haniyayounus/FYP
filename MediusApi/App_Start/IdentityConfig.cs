using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Database.Entities;
using Database.DatabaseContext;
using MeraSabaqApi.Models;
using Newtonsoft.Json;
using HttpContent = System.Net.Http.HttpContent;
using Medius.Model;
using Medius.DataAccess.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediusApi
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            const string sentFrom = "admin@sabaq.edu.pk";

            MailMessage mail = new MailMessage { From = new MailAddress(sentFrom, "Team Medius") };
            mail.To.Add(message.Destination);

            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 600000,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sentFrom, "IT11@Stub+", "sabaq.edu.pk"),
            };

            return smtp.SendMailAsync(mail);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri("http://smsctp3.eocean.us:24555") };
            string url =
                @"api?action=sendmessage&username=sabaq_api&password=pak!456&recipient=" + message.Destination +
                "&originator=99095&messagedata=" + message.Body;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await client.GetAsync(url);
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}