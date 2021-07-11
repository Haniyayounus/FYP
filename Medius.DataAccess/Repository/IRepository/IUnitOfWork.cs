using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICityRepository City { get; }
        IClaimRepository Claim { get; }
        IFAQRepository FAQ { get; }
        IIpFilterRepository IpFilter { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
