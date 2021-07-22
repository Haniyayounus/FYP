using Medius.Model;
using Medius.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IIpFilterRepository : IRepositoryAsync<IpFilter>
    {
        Task<List<IpFilter>> GetAllTechnology();
        Task<List<IpFilter>> GetAllCategory();
        Task<IpFilter> AddAsync(IpFilter entity);
        Task<IpFilter> Update(IpFilter ipFilter);
        Task<bool> IsIpFilterDuplicate(string name);
        Task<bool> IsIpFilterDuplicate(int id, string name);
        Task<List<IpFilter>> GetIpFilterByType(FilterType filterType);
    }
}
