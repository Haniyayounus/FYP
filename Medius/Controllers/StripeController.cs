using Medius.DataAccess.Repository.IRepository;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/Stripe")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly ICaseRepository _caserepo;
        private readonly IUnitOfWork _unitofwork;

        public StripeController(ICaseRepository caserepo, IUnitOfWork unitofwork)
        {
            _caserepo = caserepo;
            _unitofwork = unitofwork;
        }

        [HttpPost]
        public async Task<IActionResult> Charge(StripViewModel viewModel)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();

            try
            {
                var options = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = viewModel.CardNumber,
                        ExpMonth = viewModel.ExpMonth,
                        ExpYear = viewModel.ExpYear,
                        Cvc = viewModel.Cvc,
                    },
                };
                TokenCreateOptions token = new TokenCreateOptions();
                token.Card = options.Card;
                TokenService serviceToken = new TokenService();
                Token newToken = serviceToken.Create(token);
                viewModel.stripeToken = newToken.Id;
                var customer = customerService.Create(new CustomerCreateOptions
                {
                    Email = viewModel.stripeEmail,
                    Source = viewModel.stripeToken
                });

                string customerID = customer.Id;
                if (viewModel.mode == ModeofRegistration.Fast)
                    viewModel.amount = 2000;
                else
                    viewModel.amount = 500;

                var charge = chargeService.Create(new ChargeCreateOptions
                {
                    Amount = viewModel.amount,
                    Description = "Medius Project",
                    Currency = "usd",
                    Customer = customerID,
                    ReceiptEmail = viewModel.stripeEmail,
                    //Metadata = new Dictionary<string, string>()
                    //{
                    //    { "OrderId" , "111", },
                    //    { "Postcode" , "LEE11" },
                    //}
                });

                if (charge.Status == "succeeded")
                {
                    string BalanceTransactionId = charge.BalanceTransactionId;
                    var stipemodel = await _unitofwork.Stripe.AddAsync(viewModel);
                    _unitofwork.Save();
                    return StatusCode(StatusCodes.Status200OK, BalanceTransactionId);
                }
                else
                {
                    var data = await _caserepo.DeleteIP(viewModel.caseId);
                    return StatusCode(StatusCodes.Status200OK, data);
                }
            }
            catch (Exception ex)
            {
                // Log exception code goes here
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
