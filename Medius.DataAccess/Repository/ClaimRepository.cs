using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class ClaimRepository : RepositoryAsync<Claim>, IClaimRepository
    {
        private readonly ApplicationDbContext _db;

        public ClaimRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Claim> Update(Claim claim)
        {
            var objFromDb = _db.Claims.FirstOrDefault(s => s.Id == claim.Id) ?? throw new Exception($"No claim found against id:'{claim.Id}'");
            if (claim.Id == 0) throw new Exception($"City id cant be null");
            if (await IsClaimDuplicate(claim.Id, claim.Description)) throw new Exception($"'{claim.Description}' already exists. Please choose a different name.");

            objFromDb.Description = claim.Description;
            objFromDb.LastModify = DateTime.Now;
            return objFromDb;
        }
        public Task<bool> IsClaimDuplicate(string name)
            => _db.Claims.AnyAsync(x => x.Description.ToLower().Equals(name.ToLower()) && x.IsActive == true);

        public Task<bool> IsClaimDuplicate(int id, string name)
             => _db.Claims.AnyAsync(x => x.Description.ToLower().Equals(name.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        public async Task<Claim> AddAsync(Claim entity)
        {
            if (await IsClaimDuplicate(entity.Description)) throw new Exception($"'{entity.Description}' already exists. Please choose a different name.");
            await _db.Claims.AddAsync(entity);
            return entity;
        }
    }
}
