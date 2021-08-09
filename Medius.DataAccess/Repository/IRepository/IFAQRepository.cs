using Medius.Model;
using Medius.Model.ViewModel;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IFAQRepository : IRepositoryAsync<FAQ>
    {
        Task<FAQ> AddAsync(FAQ entity);
        Task<FAQ> Update(FAQ claim);
        Task<FAQ> Archive(ArchiveFAQVM viewModel);
        Task<bool> IsFAQDuplicate(string question);
        Task<bool> IsFAQDuplicate(int id, string question);
    }
}
