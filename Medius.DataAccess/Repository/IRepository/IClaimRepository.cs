using Medius.Model;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IClaimRepository : IRepositoryAsync<Claim>
    {
        Task<Claim> AddAsync(Claim entity);
        Task<Claim> Update(Claim claim);
        Task<bool> IsClaimDuplicate(string desc);
        Task<bool> IsClaimDuplicate(int id, string desc);
    }
}
