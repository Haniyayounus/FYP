using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;

namespace Medius.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            City = new CityRepository(_db);
            Claim = new ClaimRepository(_db);
            FAQ = new FAQRepository(_db);
            Stripe = new StripeRepository(_db);
            IpFilter = new IpFilterRepository(_db);
        }

        public ICityRepository City { get; private set; }
        public IClaimRepository Claim { get; private set; }
        public IFAQRepository FAQ { get; private set; }
        public IStripeRepository Stripe { get; private set; }
        public IIpFilterRepository IpFilter { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
