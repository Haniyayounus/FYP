using Medius.DataAccess.Repository.IRepository;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/Case")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly ICaseRepository _unitOfWork;
        public CaseController(ICaseRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllUserCases")]
        public async Task<IActionResult> GetAllUserCases(string applicationUserId)
        {
            var allObj = await _unitOfWork.GetAllUserCases(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllTrademark")]
        public async Task<IActionResult> GetAllTrademark()
        {
            var allObj = await _unitOfWork.GetAllTrademark();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllPatent")]
        public async Task<IActionResult> GetAllPatent()
        {
            var allObj = await _unitOfWork.GetAllPatent();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllDesign")]
        public async Task<IActionResult> GetAllDesign()
        {
            var allObj = await _unitOfWork.GetAllDesign();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllCopyright")]
        public async Task<IActionResult> GetAllCopyright()
        {
            var allObj = await _unitOfWork.GetAllCopyright();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserCopyright")]
        public async Task<IActionResult> GetUserCopyright(string applicationUserId)
        {
            var allObj = await _unitOfWork.GetUserCopyright(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserTrademark")]
        public async Task<IActionResult> GetUserTrademark(string applicationUserId)
        {
            var allObj = await _unitOfWork.GetUserTrademark(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserDesign")]
        public async Task<IActionResult> GetUserDesign(string applicationUserId)
        {
            var allObj = await _unitOfWork.GetUserDesign(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserPatent")]
        public async Task<IActionResult> GetUserPatent(string applicationUserId)
        {
            var allObj = await _unitOfWork.GetUserPatent(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPut]
        [Route("ChangeIPStatus")]
        public async Task<IActionResult> ChangeIPStatus(ChangeStatusViewModel viewModel)
        {
            try
            {
                if (viewModel.loggedInUserId == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Logged in user id can't be null");
                if (viewModel.userId == null)
                    return StatusCode(StatusCodes.Status404NotFound, "User id can't be null");
                if (viewModel.caseId == 0)
                    return StatusCode(StatusCodes.Status404NotFound, "Case id can't be null");
                var allObj = await _unitOfWork.ChangeIPStatus(viewModel);
                return StatusCode(StatusCodes.Status200OK, allObj);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(CaseViewModel viewModel)
        {
            try
            {
                var allObj = await _unitOfWork.Add(viewModel);
                return StatusCode(StatusCodes.Status200OK, allObj);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

    }
}
