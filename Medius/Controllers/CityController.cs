using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/City")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.City.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);

        }

        [HttpGet]
        [Route("GetById/{Id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _unitOfWork.City.GetAsync(id);
            if (allObj == null) { throw new Exception($"No City found against id:'{id}'"); }
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]

        public async Task<IActionResult> Add(CityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                City city = new City
                    {
                        Name = viewModel.Name
                    };
                    var data = await _unitOfWork.City.AddAsync(city);
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
        public async Task<IActionResult> Update(UpdateCityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                City city = new City
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name
                };
                var data = await _unitOfWork.City.Update(city);
                _unitOfWork.Save();
                return StatusCode(StatusCodes.Status200OK, data);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        [HttpDelete]
        [Route("Delete/{Id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.City.GetAsync(id);
            if (objFromDb == null) throw new Exception($"No City found against id:'{id}'"); 
            var obj = await _unitOfWork.City.RemoveAsync(id);
            _unitOfWork.Save();

            return StatusCode(StatusCodes.Status200OK, obj);
        }
    }
}
