using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Medius.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository
{
    public class CaseRepository : RepositoryAsync<Case>, ICaseRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailSender _emailService;
        private readonly IHostingEnvironment _env;
        public CaseType CaseTyoe { get; private set; }

        public CaseRepository(ApplicationDbContext db, IHostingEnvironment env, EmailSender emailService) : base(db)
        {
            _db = db;
            _env = env;
            _emailService = emailService;
        }

        public async Task<Case> Update(Case entity)
        {
            var objFromDb = _db.Cases.FirstOrDefault(s => s.Id == entity.Id) ?? throw new Exception($"No Case found against id:'{entity.Id}'");
            if (entity.Id == 0) throw new Exception($"Case id cant be null");
            if (await IsCaseDuplicate(entity.Id, entity.Title)) throw new Exception($"'{entity.Title}' already exists. Please choose a different name.");

            objFromDb.Title = entity.Title;
            objFromDb.LastModify = DateTime.Now;
            return objFromDb;
        }
        public Task<bool> IsCaseDuplicate(string title)
            => _db.Cases.AnyAsync(x => x.Title.ToLower().Equals(title.ToLower()) && x.IsActive == true);

        public Task<bool> IsCaseDuplicate(int id, string title)
             => _db.Cases.AnyAsync(x => x.Title.ToLower().Equals(title.ToLower()) && x.IsActive == true && !x.Id.Equals(id));

        public async Task<Case> Add(CaseViewModel viewModel)
        {
            if (viewModel.Title == null || viewModel.Title == "") throw new Exception($"Title is required. Please write a Title.");
            if (await IsCaseDuplicate(viewModel.Title)) throw new Exception($"'{viewModel.Title}' already exists. Please choose a different name.");

            //image upload
            var image = await CaseImages(viewModel.UserId,viewModel.Type, viewModel.Image);
            if (image == null) throw new Exception($"Image is required.");

            //document upload
            var document = await CaseDocuments(viewModel.UserId, viewModel.Type, viewModel.Document);
            if (document == null) throw new Exception($"File is required.");

            Case model = new Case()
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                Type = viewModel.Type,
                Contact = viewModel.Contact,
                Application = viewModel.Application,
                Status = Status.Sent,
                ModeofRegistration = viewModel.ModeofRegistration,
                ModifiedBy = viewModel.ModifiedBy,
                ClaimId = viewModel.ClaimId,
                CityId = viewModel.CityId,
                UserId = viewModel.UserId,
                IpFilterId = viewModel.IpFilterId,
                ImagePath = image,
                DocumentPath = document
            };
            await _db.Cases.AddAsync(model);
            await _db.SaveChangesAsync();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            sendCaseRegisterEmail(model, user.Email);
            return model;
        }
        public async Task<Case> GetCase(int id)
        {
            var objFromDb = _db.Cases.FirstOrDefault(s => s.Id == id) ?? throw new Exception($"No Case found against id:'{id}'");
            return objFromDb;
        }

        public async Task<List<Case>> GetAllTrademark()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Trademark && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllCopyright()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Copyright && x.IsActive).AsNoTracking().ToListAsync();
        }

       public async Task<List<Case>> GetAllDesign()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Design && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllPatent()
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Patent && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserTrademark(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Trademark && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserCopyright(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Copyright && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserDesign(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Design && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetUserPatent(string userId)
        {
            return await _db.Cases.Where(x => x.Type == CaseType.Patent && x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<List<Case>> GetAllUserCases(string userId)
        {
            return await _db.Cases.Where(x => x.UserId == userId && x.IsActive).AsNoTracking().ToListAsync();
        }

        public async Task<Case> ChangeIPStatus(ChangeStatusViewModel vm)
        {
            var casebyId = await _db.Cases.FirstOrDefaultAsync(x => x.UserId == vm.userId && x.Id == vm.caseId && x.IsActive);
            casebyId.Status = vm.Status;
            casebyId.ModifiedBy = vm.loggedInUserId;
            casebyId.LastModify = DateTime.Now;
            await _db.SaveChangesAsync();
            sendCaseStatusEmail(casebyId);
            return casebyId;
        }
        public void sendCaseStatusEmail(Case account)
        {
            string message;
            string path;
            string subject = "Email Verification";
            if(account.Status == Status.Reject)
                 path = Path.Combine(_env.WebRootPath, "CaseRejectionEmail.html");
            
            else if(account.Status == Status.Publish)
                path = Path.Combine(_env.WebRootPath, "CasePublishEmail.html");

            else
                path = Path.Combine(_env.WebRootPath, "CaseStatusEmail.html");
            string content = System.IO.File.ReadAllText(path);
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.Status}</code></p>";

                content = content.Replace("{{status}}", account.Status.ToString());
                content = content.Replace("{{message}}", message);
                content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            _emailService.SendEmailAsync(account.Appl.Email, subject, content);

        }

        public void sendCaseRegisterEmail(Case account, string email)
        {
            string message;
            string path;
            string subject = "Case Registration Email";

                path = Path.Combine(_env.WebRootPath, "CaseStatusEmail.html");
            string content = System.IO.File.ReadAllText(path);
            message = $@"<p>Hurrah!! you have successfully registered your case</p>
                             <p><code>{account.Status}</code></p>";

            content = content.Replace("{{status}}", account.Status.ToString());
            content = content.Replace("{{message}}", message);
            content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            _emailService.SendEmailAsync(email, subject, content);

        }

        public async Task<Case> DeleteIP(int id)
        {
            var userCase = await GetCase(id);
            _db.Cases.Remove(userCase);
            await _db.SaveChangesAsync();
            return userCase;
        }

        //Image upload
        public async Task<Images> UploadedImage(CaseType ipType, IFormFile image)
        {
            string filePath = null;
            if (ipType == CaseType.Patent)
                filePath = Path.Combine(_env.WebRootPath, "CaseImages", "Patent");
            else if (ipType == CaseType.Copyright)
                filePath = Path.Combine(_env.WebRootPath, "CaseImages", "Copyright");
            else if (ipType == CaseType.Trademark)
                filePath = Path.Combine(_env.WebRootPath, "CaseImages", "Trademark");
            else if (ipType == CaseType.Design)
                filePath = Path.Combine(_env.WebRootPath, "CaseImages", "Design");
            string fileName;
            Images images = new Images();

            try
            {
                var extension = "." + image.FileName.Split('.')[image.FileName.Split('.').Length - 1];
                fileName = image.FileName + extension;

                string uniqueFileName = null;

                if (image != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, filePath);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }

                images = new Images
                {
                    ImagePath = filePath,
                    Image = image
                };
            }
            catch (Exception e)
            {
                //log error
                e.Message.ToString();
            }

            return images;
        }

        //document upload
        public async Task<Document> UploadFile(CaseType ipType, IFormFile file)
        {
            var filePath = "";
            if (ipType == CaseType.Patent)
                filePath = Path.Combine(_env.WebRootPath, "Documents", "Patent");
            else if (ipType == CaseType.Copyright)
                filePath = Path.Combine(_env.WebRootPath, "Documents", "Copyright");
            else if (ipType == CaseType.Trademark)
                filePath = Path.Combine(_env.WebRootPath, "Documents", "Trademark");
            else if (ipType == CaseType.Design)
                filePath = Path.Combine(_env.WebRootPath, "Documents", "Design");

            string fileName;
            Document document = new Document();
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                //fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.
                fileName = file.FileName;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = Path.Combine(filePath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                document = new Document
                {
                    DocumentPath = path,
                    FileDocument = file
                };
            }
            catch (Exception e)
            {
                //log error
                e.Message.ToString();
            }

            return document;
        }


        private async Task<string> CaseImages(string id, CaseType ipType, IFormFile image)
        {
            //get user by id
            var user = _db.Users.FirstOrDefault(m => m.Id == id);
            AmazonUploader myUploader = new AmazonUploader();
            bool a;
            string myBucketName = "dinematebucket"; //your s3 bucket name goes here  
            var date = DateTime.Now;
            //get random number
            var randomNumber = new TimeSpan(date.Year, date.Month, date.Day, new Random().Next(10000)).TotalSeconds;

            var imageName = randomNumber + "-" + ipType + "-" + user.UserName + "- Image";

            if (FileUtility.IsImageFile(image.FileName) == false)
            {
                throw new Exception("Invalid File Type");
            }
            var st = new MemoryStream();
            await image.CopyToAsync(st);
            var fileBytes = st.ToArray();
            string s = Convert.ToBase64String(fileBytes);
            // act on the Base64 data
            //Stream st = file.PostefFile.InputStream;
            string name = Path.GetFileName(image.FileName);
            string s3DirectoryName = "CaseImages";
            name = imageName + ".png";
            string s3FileName = @name;
            a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
            if (a == true)
            {
                var baseUrl = "https://dinematebucket.s3.us-east-2.amazonaws.com/CaseImages/" + name;
                return baseUrl;
            }
            else
            {
                return null;
            }
        }

        private async Task<string> CaseDocuments(string id, CaseType ipType, IFormFile file)
        {
            //get user by id
            var user = _db.Users.FirstOrDefault(m => m.Id == id);
            AmazonUploader myUploader = new AmazonUploader();
            bool a;
            string myBucketName = "dinematebucket"; //your s3 bucket name goes here  
            var date = DateTime.Now;
            //get random number
            var randomNumber = new TimeSpan(date.Year, date.Month, date.Day, new Random().Next(10000)).TotalSeconds;

            var fileName = randomNumber + "-" + ipType + "-" + user.UserName;

            if (FileUtility.IsDocumentFile(file.FileName) == false)
            {
                throw new Exception("Invalid File Type");
            }

            var st = new FileStream(fileName, FileMode.Create);
                    await file.CopyToAsync(st);
            //var st = new MemoryStream();
            //await file.CopyToAsync(st);
            //var fileBytes = st.ToArray();

            //string s = Convert.ToBase64String(fileBytes);
            // act on the Base64 data
            string name = Path.GetFileName(file.FileName);
            string s3DirectoryName = "CaseDocuments";
            name = fileName + ".pdf";
            string s3FileName = @name;
            a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
            if (a == true)
            {
                var baseUrl = "https://dinematebucket.s3.us-east-2.amazonaws.com/CaseDocuments/" + name;
                return baseUrl;
            }
            else
            {
                return null;
            }
        }
    }
}
