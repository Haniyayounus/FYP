using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
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
    public class CaseRepository : RepositoryAsync<Case>, ICaseRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailSender _emailService;
        private readonly IHostingEnvironment _env;
        public CaseType CaseTyoe { get; private set; }

        public CaseRepository(ApplicationDbContext db, IHostingEnvironment env, EmailSender emailService) : base(db)
        {
            _db = db;
            _env = env;
            _emailService = emailService;
        }

        public async Task<Case> Update(Case entity)
        {
            var objFromDb = _db.Cases.FirstOrDefault(s => s.Id == entity.Id) ?? throw new Exception($"No Case found against id:'{entity.Id}'");
            if (entity.Id == 0) throw new Exception($"Case id cant be null");
            if (await IsCaseDuplicate(entity.Id, entity.Title)) throw new Exception($"'{entity.Title}' already exists. Please choose a different name.");

            objFromDb.Title = entity.Title;
            objFromDb.LastModify = DateTime.Now;
            return objFromDb;
        }
        public Task<bool> IsCaseDuplicate(string title)
            => _db.Cases.AnyAsync(x => x.Title.ToLower().Equals(title.ToLower()) && x.IsActive == true);

        public Task<bool> IsCaseDuplicate(int id, string title)
             => _db.Cases.AnyAsync(x => x.Title.ToLower().Equals(title.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        public async Task<Case> AddAsync(Case entity)
        {
            if (await IsCaseDuplicate(entity.Title)) throw new Exception($"'{entity.Title}' already exists. Please choose a different name.");
            await _db.Cases.AddAsync(entity);
            return entity;
        }
        public async Task<Case> GetCase(int id)
        {
            var objFromDb = _db.Cases.FirstOrDefault(s => s.Id == id) ?? throw new Exception($"No Case found against id:'{id}'");
            return objFromDb;
        }

        public async Task<List<Case>> GetAllTrademark()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Trademark && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllCopyright()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Copyright && x.IsActive).AsNoTracking().ToListAsync();
        }

       public async Task<List<Case>> GetAllDesign()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Design && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllPatent()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Patent && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserTrademark(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Trademark && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserCopyright(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Copyright && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserDesign(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Design && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserPatent(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Patent && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllUserCases(string userId)
        {
            return await _db.Cases.Where(x => x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<Case> ChangeIPStatus(ChangeStatusViewModel vm)
        {
            var casebyId = await _db.Cases.FirstOrDefaultAsync(x => x.UserId == vm.userId && x.Id == vm.caseId && x.IsActive);
            casebyId.Status = vm.Status;
            casebyId.ModifiedBy = vm.loggedInUserId;
            casebyId.LastModify = DateTime.Now;
            await _db.SaveChangesAsync();
            sendCaseStatusEmail(casebyId);
            return casebyId;
        }
        public void sendCaseStatusEmail(Case account)
        {
            string message;
            string path;
            string subject = "Email Verification";
            if(account.Status == Status.Reject)
                 path = Path.Combine(_env.WebRootPath, "CaseRejectionEmail.html");
            else
                 path = Path.Combine(_env.WebRootPath, "CaseStatusEmail.html");
            string content = System.IO.File.ReadAllText(path);
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.Status}</code></p>";

                content = content.Replace("{{status}}", account.Status.ToString());
                content = content.Replace("{{message}}", message);
                content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            _emailService.SendEmailAsync(account.Appl.Email, subject, content);

        }

    }
}
