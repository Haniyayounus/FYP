using Medius.Model;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IClaimRepository : IRepositoryAsync<Claims>
    {
        Task<Claims> AddAsync(Claims entity);
        Task<Claims> Update(Claims claim);
        Task<bool> IsClaimDuplicate(string desc);
        Task<bool> IsClaimDuplicate(int id, string desc);
    }
}
