using Medius.DataAccess.Data;
using Medius.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Medius
{

    //public class EmailService : IIdentityMessageService
    //{
    //    public Task SendAsync(IdentityMessage message)
    //    {
    //        const string sentFrom = "medisudigitalaccess@gmail.com";

    //        MailMessage mail = new MailMessage { From = new MailAddress(sentFrom, "Team Medius") };
    //        mail.To.Add(message.Destination);

    //        mail.Subject = message.Subject;
    //        mail.Body = message.Body;
    //        mail.IsBodyHtml = true;
    //        SmtpClient smtp = new SmtpClient("smtp.office365.com", 587)
    //        {
    //            EnableSsl = true,
    //            DeliveryMethod = SmtpDeliveryMethod.Network,
    //            Timeout = 600000,
    //            UseDefaultCredentials = false,
    //            Credentials = new NetworkCredential(sentFrom),
    //        };

    //        return smtp.SendMailAsync(mail);
    //    }
    //}


    //    public class ApplicationUserManager : UserManager<ApplicationUser>
    //    {
    //        public ApplicationUserManager(IUserStore<ApplicationUser> store)
    //            : base(store)
    //        {
    //        }

    //        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    //        {
    //            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
    //            // Configure validation logic for usernames
    //            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
    //            {
    //                AllowOnlyAlphanumericUserNames = false,
    //                RequireUniqueEmail = false
    //            };
    //            // Configure validation logic for passwords
    //            manager.PasswordValidator = new PasswordValidator
    //            {
    //                RequiredLength = 6,
    //                RequireNonLetterOrDigit = false,
    //                RequireDigit = false,
    //                RequireLowercase = false,
    //                RequireUppercase = false,
    //            };
    //            var dataProtectionProvider = options.DataProtectionProvider;
    //            if (dataProtectionProvider != null)
    //            {
    //                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
    //            }
    //            return manager;
    //        }
    //    }
}
