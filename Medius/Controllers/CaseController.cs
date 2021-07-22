using Medius.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/Case")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.Case.GetAllAsync();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllUserCases")]
        public async Task<IActionResult> GetAllUserCases(string applicationUserId)
        {
            var allObj = await _unitOfWork.Case.GetAllUserCases(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllTrademark")]
        public async Task<IActionResult> GetAllTrademark()
        {
            var allObj = await _unitOfWork.Case.GetAllTrademark();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllPatent")]
        public async Task<IActionResult> GetAllPatent()
        {
            var allObj = await _unitOfWork.Case.GetAllPatent();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllDesign")]
        public async Task<IActionResult> GetAllDesign()
        {
            var allObj = await _unitOfWork.Case.GetAllDesign();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetAllCopyright")]
        public async Task<IActionResult> GetAllCopyright()
        {
            var allObj = await _unitOfWork.Case.GetAllCopyright();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserCopyright")]
        public async Task<IActionResult> GetUserCopyright(string applicationUserId)
        {
            var allObj = await _unitOfWork.Case.GetUserCopyright(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserTrademark")]
        public async Task<IActionResult> GetUserTrademark(string applicationUserId)
        {
            var allObj = await _unitOfWork.Case.GetUserTrademark(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserDesign")]
        public async Task<IActionResult> GetUserDesign(string applicationUserId)
        {
            var allObj = await _unitOfWork.Case.GetUserDesign(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetUserPatent")]
        public async Task<IActionResult> GetUserPatent(string applicationUserId)
        {
            var allObj = await _unitOfWork.Case.GetUserPatent(applicationUserId);
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

    }
}
