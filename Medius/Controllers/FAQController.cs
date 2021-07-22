using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/FAQ")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public FAQController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.FAQ.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);

        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _unitOfWork.FAQ.GetAsync(id);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(FAQViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                FAQ faq = new FAQ
                {
                    Question = viewModel.Question,
                    Answer = viewModel.Answer
                };
                var data = await _unitOfWork.FAQ.AddAsync(faq);
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
        public async Task<IActionResult> Update(FAQViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                FAQ faq = new FAQ
                {
                    Id = viewModel.Id,
                    Question = viewModel.Question,
                    Answer = viewModel.Answer
                };
                var data = await _unitOfWork.FAQ.Update(faq);
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
            var objFromDb = await _unitOfWork.FAQ.GetAsync(id);
            if (objFromDb == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await _unitOfWork.FAQ.RemoveAsync(objFromDb);
            _unitOfWork.Save();

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

