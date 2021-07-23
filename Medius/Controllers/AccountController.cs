using AutoMapper.Configuration;
using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Medius.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Medius.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseController
    {
            private readonly IApplicationUserRepository _unitOfWork;
            private readonly ApplicationDbContext _db;
            private readonly IMapper _mapper; 
        private readonly IHostingEnvironment _env;
        private readonly EmailSender _emailService;


        public AccountController(IApplicationUserRepository unitOfWork, IMapper mapper, ApplicationDbContext db,
            IHostingEnvironment env, EmailSender emailService)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _db = db;
            _env = env;
            _emailService = emailService;
            }

            [HttpPost("Login")]
            public ActionResult<AuthenticateResponse> Login(AuthenticateRequest model)
            {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid Parameters.");
            }
            var response = _unitOfWork.Authenticate(model, ipAddress());
                setTokenCookie(response.RefreshToken);
                return Ok(response);
            }

            [HttpPost("refresh-token")]
            public ActionResult<AuthenticateResponse> RefreshToken()
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var response = _unitOfWork.RefreshToken(refreshToken, ipAddress());
                setTokenCookie(response.RefreshToken);
                return Ok(response);
            }

            [Authorize]
            [HttpPost("revoke-token")]
            public IActionResult RevokeToken(RevokeTokenRequest model)
            {
                // accept token from request body or cookie
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Token is required" });

                // users can revoke their own tokens and admins can revoke any tokens
                if (!ApplicationUser.OwnsToken(token) && ApplicationUser.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

            _unitOfWork.RevokeToken(token, ipAddress());
                return Ok(new { message = "Token revoked" });
            }

            [HttpPost("Register")]
            public IActionResult Register(RegisterRequest model)
        {
            
            // validate
            if (_db.ApplicationUsers.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                return Ok(
                    new { 
                        message = "Email already Registered" });

            }
            var account = _unitOfWork.Register(model, Request.Headers["origin"]);
            ////Email Service
            //EmailService service = new EmailService();
            //string path = Path.Combine(_env.WebRootPath, "/VerificationCode.html");
            //string content = System.IO.File.ReadAllText(path);
            //content = content.Replace("{{resetToken}}", account.VerificationToken);
            //content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());
            //string subject = "Hello and Welcome to Medius !";

            //_emailService.SendEmailAsync(content, subject, model.Email);
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
            }

            [HttpPost("verify-email")]
            public IActionResult VerifyEmail(VerifyEmailRequest model)
            {
            _unitOfWork.VerifyEmail(model.Token);
                return Ok(new { message = "Verification successful, you can now login" });
            }

            [HttpPost("ForgotPassword")]
            public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
            {
            if (model.mode == "SMS")
            {
                var code = StringUtility.GenerateVerificationCode(4);
                var account = await _db.ApplicationUsers.SingleOrDefaultAsync(x => x.Email == model.Email);

                // always return ok response to prevent email enumeration
                if (account == null) return null;

                // create reset token that expires after 1 day
                account.ResetToken = code;
                account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

                _db.ApplicationUsers.Update(account);
                _db.SaveChanges();
                // SMS Service
                var accountSid = ("ACe7643b8eb95e15efa182bffdfca15d15");
                var authToken = ("335edde3ed8e23507017646fb6061fce");
                var from = ("+17032918257");
                var to = account.PhoneNumber;
                TwilioClient.Init(accountSid, authToken);

                var message = await MessageResource.CreateAsync
                    (
                    to: to,
                    from: from,
                    body: "Your medius security code is " + account.ResetToken
                    );
                return StatusCode(StatusCodes.Status200OK, code);
            }
            else
            {
                // Email Service

                var resetToken =_unitOfWork.ForgotPassword(model, Request.Headers["origin"]);
                var blob = new JObject(
                    new JProperty("VerificationCode", resetToken)
                    );
                return StatusCode(StatusCodes.Status205ResetContent, blob);
                //return Ok(new { message = "Please check your email for password reset instructions" });
            }
        }

            [HttpPost("validate-reset-token")]
            public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
            {
            _unitOfWork.ValidateResetToken(model);
                return Ok(new { message = "Token is valid" });
            }

            [HttpPost("reset-password")]
            public IActionResult ResetPassword(ResetPasswordRequest model)
            {
            _unitOfWork.ResetPassword(model);
                return Ok(new { message = "Password reset successful, you can now login" });
            }

            [Authorize(Role.Admin)]
            [HttpGet]
            public ActionResult<IEnumerable<AccountResponse>> GetAll()
            {
                var accounts = _unitOfWork.GetAll();
                return Ok(accounts);
            }

            [Authorize]
            [HttpGet("{id:int}")]
            public ActionResult<AccountResponse> GetById(string id)
            {
                // users can get their own account and admins can get any account
                if (id != ApplicationUser.Id && ApplicationUser.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

                var account = _unitOfWork.GetById(id);
                return Ok(account);
            }

            [Authorize(Role.Admin)]
            [HttpPost]
            public ActionResult<AccountResponse> Create(CreateRequest model)
            {
                var account = _unitOfWork.Create(model);
                return Ok(account);
            }

            [Authorize]
            [HttpPut("{id:int}")]
            public ActionResult<AccountResponse> Update(string id, UpdateRequest model)
            {
                // users can update their own account and admins can update any account
                if (id != ApplicationUser.Id && ApplicationUser.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

                // only admins can update role
                if (ApplicationUser.Role != Role.Admin)
                    model.Role = null;

                var account = _unitOfWork.Update(id, model);
                return Ok(account);
            }

            [Authorize]
            [HttpDelete("{string:int}")]
            public IActionResult Delete(string id)
            {
                // users can delete their own account and admins can delete any account
                if (id != ApplicationUser.Id && ApplicationUser.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

            _unitOfWork.Delete(id);
                return Ok(new { message = "Account deleted successfully" });
            }

            // helper methods

            private void setTokenCookie(string token)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", token, cookieOptions);
            }

            private string ipAddress()
            {
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    return Request.Headers["X-Forwarded-For"];
                else
                    return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
    }
}
