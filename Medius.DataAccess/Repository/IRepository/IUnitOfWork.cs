using System;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICityRepository City { get; }
        IClaimRepository Claim { get; }
        IFAQRepository FAQ { get; }
        IIpFilterRepository IpFilter { get; }
        void Save();
    }
}
