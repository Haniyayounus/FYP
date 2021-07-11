using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Route("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _unitOfWork.City.GetAsync(id);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]

        public async Task<IActionResult> Add(string cityName)
        {
            if (ModelState.IsValid)
            {
                City city = new City
                    {
                        Name = cityName
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
        public async Task<IActionResult> Update(int id, string cityName)
        {
            if (ModelState.IsValid)
            {
                City city = new City
                {
                    Id = id,
                    Name = cityName
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
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.City.GetAsync(id);
            if (objFromDb == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await _unitOfWork.City.RemoveAsync(objFromDb);
            _unitOfWork.Save();

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
