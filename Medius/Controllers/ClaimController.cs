using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/Claim")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClaimController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.Claim.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);

        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _unitOfWork.Claim.GetAsync(id);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(ClaimViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Claims claim = new Claims
                {
                    Description = viewModel.Description
                };
                var data = await _unitOfWork.Claim.AddAsync(claim);
                _unitOfWork.Save();
                return StatusCode(StatusCodes.Status200OK, data);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(UpdateClaimViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Claims claim = new Claims
                {
                    Id = viewModel.Id,
                    Description = viewModel.Description
                };
                var data = await _unitOfWork.Claim.Update(claim);
                _unitOfWork.Save();
                return StatusCode(StatusCodes.Status200OK, data);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.Claim.GetAsync(id);
            if (objFromDb == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await _unitOfWork.Claim.RemoveAsync(objFromDb);
            _unitOfWork.Save();

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
