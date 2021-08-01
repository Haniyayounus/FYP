using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using System;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class StripeRepository : RepositoryAsync<StripePayment>, IStripeRepository
    {
        private readonly ApplicationDbContext _db;
        public StripeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<StripePayment> AddAsync(StripViewModel viewModel)
        {
            StripePayment model = new StripePayment()
            {
                Email = viewModel.stripeEmail,
                Token = viewModel.stripeToken,
                CaseId = viewModel.caseId,
                UserId = viewModel.userId,
                Amount = viewModel.amount
            };
            _db.StripePayments.Add(model);
            return model;
        }
    }
}
