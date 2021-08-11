using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/ProfilePicture")]
    [ApiController]
    public class ProfilePictureController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProfilePictureController(IUnitOfWork unitOfWork, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
        }
        [HttpPost]
        [Route("UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImage(string id, IFormFile file)
        {
            if (id == null || id == "")
                return StatusCode(StatusCodes.Status404NotFound, "UserId can't be null");

            if (file == null || file.Length == 0)
                return StatusCode(StatusCodes.Status404NotFound, "File is empty");
        try{
                //get user by id
                var user = _db.Users.FirstOrDefault(m => m.Id == id);

            if(user != null && file.Length > 0)
            {
                    AmazonUploader myUploader = new AmazonUploader();
                    bool a;
                    string myBucketName = "dinematebucket"; //your s3 bucket name goes here  
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

                    //delete previous image
                    if (!string.IsNullOrEmpty(user.ImagePath))
                    {
                        try
                        {
                            string keyFilename = user.ImagePath;
                            keyFilename = keyFilename.Replace("https://dinematebucket.s3.us-east-2.amazonaws.com/","");
                           a = await myUploader.DeleteFile(keyFilename, myBucketName);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                        }
                    }

                    //string filePath = Path.Combine(_env.ContentRootPath, imageName + ".png");
                    var st = new MemoryStream();
                    await file.CopyToAsync(st);
                    var fileBytes = st.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    //Stream st = file.PostefFile.InputStream;
                    string name = Path.GetFileName(file.FileName);
                    string s3DirectoryName = "ProfileImages";
                    name = imageName + ".png";
                    string s3FileName = @name;
                    a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
                    if (a == true)
                    {
                        var baseUrl = "https://dinematebucket.s3.us-east-2.amazonaws.com/ProfileImages/" + name;
                        if (user != null)
                        {
                            user.ImagePath = baseUrl;
                            _db.Entry(user).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    return StatusCode(StatusCodes.Status200OK, "Profile picture updated successfully");
                }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, "Error");

                    }
                    //delete previous image
                    //if (!string.IsNullOrEmpty(user.ImagePath) && System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + user.ImagePath))
                    //{
                    //    try
                    //    {
                    //        var path = AppDomain.CurrentDomain.BaseDirectory + user.ImagePath;
                    //        System.IO.File.Delete(path);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        var error = new
                    //        {
                    //            message = ex.Message,
                    //            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    //        };
                    //        return StatusCode(StatusCodes.Status400BadRequest, error);
                    //    }
                    //}
                    // delete image if it exist with the same name
                    //    if (System.IO.File.Exists(filePath))
                    //{
                    //    System.IO.File.Delete(filePath);
                    //}

                    // convert string to stream
                    //using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await file.CopyToAsync(fileStream);
                    //}
                    }
                else if(user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User Not Found");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteImage(string filename)
        {
            try
            {
                string myBucketName = "dinematebucket"; //your s3 bucket name goes here  
                AmazonUploader myUploader = new AmazonUploader();
                bool a = await myUploader.DeleteFile(filename, myBucketName);
                if (a)
                {
                    return StatusCode(StatusCodes.Status200OK, "Deleted");

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}