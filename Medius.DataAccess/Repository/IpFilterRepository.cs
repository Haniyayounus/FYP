using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class IpFilterRepository : RepositoryAsync<IpFilter>, IIpFilterRepository
    {
        private readonly ApplicationDbContext _db;

        public IpFilterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IpFilter> Update(IpFilter ipFilter)
        {
            var objFromDb = _db.IpFilters.FirstOrDefault(s => s.Id == ipFilter.Id) ?? throw new Exception($"No Filter found against id:'{ipFilter.Id}'");
            if (ipFilter.Id == 0) throw new Exception($"Filter id cant be null");
            if (await IsIpFilterDuplicate(ipFilter.Id, ipFilter.Name)) throw new Exception($"'{ipFilter.Name}' already exists. Please choose a different name.");

            objFromDb.Name = ipFilter.Name;
            objFromDb.LastModify = DateTime.Now;
            return objFromDb;
        }
        public Task<bool> IsIpFilterDuplicate(string name)
            => _db.IpFilters.AnyAsync(x => x.Name.ToLower().Equals(name.ToLower()) && x.IsActive == true);

        public Task<bool> IsIpFilterDuplicate(int id, string name)
             => _db.IpFilters.AnyAsync(x => x.Name.ToLower().Equals(name.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        public async Task<IpFilter> AddAsync(IpFilter entity)
        {
            if (await IsIpFilterDuplicate(entity.Name)) throw new Exception($"'{entity.Name}' already exists. Please choose a different name.");
            await _db.IpFilters.AddAsync(entity);
            return entity;
        }
    }
}
