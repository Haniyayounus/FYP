using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/ProfilePicture")]
    [ApiController]
    public class ProfilePictureController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProfilePictureController(IUnitOfWork unitOfWork, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _env = env;
        }

        [HttpPost, Route("api/ProfilePicture/UploadProfileImage/")]
        public async Task<IActionResult> UploadProfileImage(string id, IFormFile file)
        {
            //get user by email
            var user = _db.Users.FirstOrDefault(m => m.Id == id);

            if (user != null && file.Length > 0)
            {
                string folderpath = Path.Combine(_env.WebRootPath, "ProfileImages");
                FileUtility.CreateFileFolder(folderpath);

                var date = DateTime.Now;
                //get random number
                var randomNumber = new TimeSpan(date.Year, date.Month, date.Day, new Random().Next(10000)).TotalSeconds;

                var imageName = randomNumber + "-" + user.UserName;

                if (FileUtility.IsImageFile(file.FileName) == false)
                {
                    var error = new
                    {
                        message = "Invalid File Type",
                        error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }

                string filePath = Path.Combine(_env.ContentRootPath, folderpath, imageName + ".png");

                //delete previous image
                if (!string.IsNullOrEmpty(user.ImagePath) && System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + user.ImagePath))
                {
                    try
                    {
                        var path = AppDomain.CurrentDomain.BaseDirectory + user.ImagePath;
                        System.IO.File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        var error = new
                        {
                            message = ex.Message,
                            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return StatusCode(StatusCodes.Status400BadRequest, error);
                    }
                }
                // delete image if it exist with the same name
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(filePath);
                //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                MemoryStream stream = new MemoryStream(byteArray);
                await file.CopyToAsync(stream);

                if (user != null)
                {
                    user.ImagePath = folderpath + imageName + ".png";
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}