using Medius.Model;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IIpFilterRepository : IRepositoryAsync<IpFilter>
    {
        Task<IpFilter> AddAsync(IpFilter entity);
        Task<IpFilter> Update(IpFilter ipFilter);
        Task<bool> IsIpFilterDuplicate(string name);
        Task<bool> IsIpFilterDuplicate(int id, string name);
    }
}
