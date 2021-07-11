using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class FAQRepository : RepositoryAsync<FAQ>, IFAQRepository
    {
        private readonly ApplicationDbContext _db;

        public FAQRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<FAQ> Update(FAQ faq)
        {
            var objFromDb = _db.FAQs.FirstOrDefault(s => s.Id == faq.Id) ?? throw new Exception($"No Question found against id:'{faq.Id}'");
            if (faq.Id == 0) throw new Exception($"FAQ id cant be null");
            if (await IsFAQDuplicate(faq.Id, faq.Question)) throw new Exception($"'{faq.Question}' already exists. Please choose a different name.");

            objFromDb.Question = faq.Question;
            objFromDb.LastModify = DateTime.Now;
            return objFromDb;
        }
        public Task<bool> IsFAQDuplicate(string question)
            => _db.FAQs.AnyAsync(x => x.Question.ToLower().Equals(question.ToLower()) && x.IsActive == true);

        public Task<bool> IsFAQDuplicate(int id, string question)
             => _db.FAQs.AnyAsync(x => x.Question.ToLower().Equals(question.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        public async Task<FAQ> AddAsync(FAQ entity)
        {
            if (await IsFAQDuplicate(entity.Question)) throw new Exception($"'{entity.Question}' already exists. Please choose a different name.");
            await _db.FAQs.AddAsync(entity);
            return entity;
        }
    }
}
