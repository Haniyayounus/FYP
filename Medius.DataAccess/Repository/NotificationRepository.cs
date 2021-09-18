using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly EmailSender _emailService;

        public NotificationRepository(ApplicationDbContext db, IWebHostEnvironment env, EmailSender emailService)
        {
            _db = db;
            _env = env;
            _emailService = emailService;
        }
        public async Task<Notification> AddAsync(Notification entity)
        {
            var emails = _db.ApplicationUsers.Select(x => x.Email).ToList();
            await _db.Notifications.AddAsync(entity);
            await _db.SaveChangesAsync();
            sendVerificationEmail(entity);
            return entity;
        }

        public async Task<List<Notification>> GetAll()
        {
            return await _db.Notifications.AsNoTracking().ToListAsync();
        }

        public async Task<Notification> GetById(int id)
        {
            return await _db.Notifications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        }

         private void sendVerificationEmail(Notification notification)
        {
            //string message;
            string subject;
            string path;
            string content;


            subject = notification.Subject;
            path = Path.Combine(_env.WebRootPath, "Notification.html");
            content = System.IO.File.ReadAllText(path);

            //message = $@"<p>Please click the below link to verify your email address:</p>
            //         <p><a href=""{resetToken}"">{resetToken}</a></p>";
            content = content.Replace("{{Title}}", notification.Title);
            content = content.Replace("{{Description}}", notification.Description);
            //content = content.Replace("{{message}}", message);
            content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            var emails = _db.Users.Select(x => x.Email).ToList();
            _emailService.SendEmailToAllAsync(emails, subject, content);

        }

    }
}
