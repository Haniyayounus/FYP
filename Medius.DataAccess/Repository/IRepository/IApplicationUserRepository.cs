using Medius.Model;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepositoryAsync<ApplicationUser>
    {
        //Task<ApplicationUser> AddAsync(ApplicationUser entity);
        //Task<ApplicationUser> Update(ApplicationUser city);
        //Task<bool> IsApplicationUserDuplicate(string name);
        //Task<bool> IsApplicationUserDuplicate(int id, string name);
    }
}
