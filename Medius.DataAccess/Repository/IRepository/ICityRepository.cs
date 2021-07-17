using Medius.Model;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface ICityRepository : IRepositoryAsync<City>
    {
        Task<City> AddAsync(City entity);
        Task<City> Update(City city);
        Task<City> GetCity(int id);
        Task<bool> IsCityDuplicate(string name);
        Task<bool> IsCityDuplicate(int id, string name);
    }
}
