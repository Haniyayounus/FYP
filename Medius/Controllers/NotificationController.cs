using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/Notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ApplicationDbContext _db;
        public NotificationController(INotificationRepository notificationRepository, ApplicationDbContext db)
        {
            _notificationRepository = notificationRepository;
            _db = db;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _notificationRepository.GetAll();
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpGet]
        [Route("GetById/{Id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var allObj = await _notificationRepository.GetById(id);
            if (allObj == null) { throw new Exception($"No Notification found against id:'{id}'"); }
            return StatusCode(StatusCodes.Status200OK, allObj);
        }

        [HttpPost]
        [Route("Add")]

        public async Task<IActionResult> Add(NotificationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Notification notification = new Notification
                {
                    Title = viewModel.Title,
                    Subject = viewModel.Subject,
                    Description = viewModel.Description,
                };
                var data = await _notificationRepository.AddAsync(notification, viewModel.Role);
                return StatusCode(StatusCodes.Status200OK, data);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }
        [HttpGet]
        [Route("GetNotificationForUser")]
        public async Task<IActionResult> GetNotificationForUser()
        {
            var obj = await _notificationRepository.GetNotificationForUser();
            return Ok(obj);
        }
    }
}
