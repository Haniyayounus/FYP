using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class ApplicationUserRepository : RepositoryAsync<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public async Task<City> Update(City city)
        //{
        //    var objFromDb = _db.Cities.FirstOrDefault(s => s.Id == city.Id) ?? throw new Exception($"No City found against id:'{city.Id}'");
        //    if(city.Id == 0) throw new Exception($"City id cant be null");
        //    if (await IsCityDuplicate(city.Id, city.Name)) throw new Exception($"'{city.Name}' already exists. Please choose a different name.");

        //        objFromDb.Name = city.Name;
        //        objFromDb.LastModify = DateTime.Now;
        //        return objFromDb;
        //}
        //public Task<bool> IsCityDuplicate(string name)
        //    => _db.Cities.AnyAsync(x => x.Name.ToLower().Equals(name.ToLower()) && x.IsActive == true);

        //public Task<bool> IsCityDuplicate(int id, string name)
        //     => _db.Cities.AnyAsync(x => x.Name.ToLower().Equals(name.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        //public async Task<City> AddAsync(City entity)
        //{
        //    if (await IsCityDuplicate(entity.Name)) throw new Exception($"'{entity.Name}' already exists. Please choose a different name.");
        //    await _db.Cities.AddAsync(entity);
        //    return entity;
        //}
    }
}
