using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/IpFilter")]
    [ApiController]
    public class IpFilterController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IpFilterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.IpFilter.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);

        }
        
        [HttpGet]
        [Route("GetAllTechnology")]
        public async Task<IActionResult> GetAllTechnology()
        {
            var allObj = await _unitOfWork.IpFilter.GetAllTechnology();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            var allObj = await _unitOfWork.IpFilter.GetAllCategory();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _unitOfWork.IpFilter.GetAsync(id);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(IPFilterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IpFilter ipFilter = new IpFilter
                {
                    Type = viewModel.Type,
                    Name = viewModel.Name

                };
                var data = await _unitOfWork.IpFilter.AddAsync(ipFilter);
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
        public async Task<IActionResult> Update(UpdateIPFilterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IpFilter ipFilter = new IpFilter
                {
                    Id = viewModel.Id,
                    Type = viewModel.Type,
                    Name = viewModel.Name
                };
                var data = await _unitOfWork.IpFilter.Update(ipFilter);
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
            var objFromDb = await _unitOfWork.IpFilter.GetAsync(id);
            if (objFromDb == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await _unitOfWork.IpFilter.RemoveAsync(objFromDb);
            _unitOfWork.Save();

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
