using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class CaseRepository : RepositoryAsync<Case>, ICaseRepository
    {
        private readonly ApplicationDbContext _db;

        public CaseType CaseTyoe { get; private set; }

        public CaseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
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
    }
}
