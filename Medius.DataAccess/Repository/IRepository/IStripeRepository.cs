using Medius.Model;
using Medius.Model.ViewModels;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IStripeRepository : IRepositoryAsync<StripePayment>
    {
        Task<StripePayment> AddAsync(StripViewModel viewModel);

    }
}
