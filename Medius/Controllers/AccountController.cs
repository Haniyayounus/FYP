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
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace Medius.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseController
    {
            private readonly IApplicationUserRepository _unitOfWork;
            private readonly ApplicationDbContext _db;


        public AccountController(IApplicationUserRepository unitOfWork, ApplicationDbContext db)
            {
                _unitOfWork = unitOfWork;
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

            [HttpPost("RefreshToken")]
            public ActionResult<AuthenticateResponse> RefreshToken()
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var response = _unitOfWork.RefreshToken(refreshToken, ipAddress());
                setTokenCookie(response.RefreshToken);
                return Ok(response);
            }

            [Authorize]
            [HttpPost("RevokeToken")]
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
                return Ok(
                    new { 
                        message = "Email already Registered" });

            }
            var account = _unitOfWork.Register(model, Request.Headers["origin"]);
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
            }

            [HttpGet("VerifyEmail")]
            public IActionResult VerifyEmail(string token)
            {
            _unitOfWork.VerifyEmail(token);
                return Ok(new { message = "Verification successful, you can now login" });
            }

            [HttpPost("ForgotPassword")]
            public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
            {
            if (model.mode == "SMS")
            {
                var code = StringUtility.GenerateVerificationCode(6);
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
                var authToken = ("dfab050cf9cbf3e5db5eae6842aa65d4");
                var from = ("+14848044359");
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

            [Authorize]
            [HttpPut]
            [Route("ChangePassword")]
            public async Task<IActionResult> ChangePassword(ChangePassword model)
            {
                try
                {
                    var data = await _unitOfWork.ChangePassword(model);
                    return StatusCode(StatusCodes.Status200OK, "Password change successfully");
                }
                catch (Exception ex)
                {
                    // Log exception code goes here
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            
            [HttpPost("ValidateResetToken")]
            public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
            {
            _unitOfWork.ValidateResetToken(model);
                return Ok(new { message = "Token is valid" });
            }

            [HttpPost("ResetPassword")]
            public IActionResult ResetPassword(ResetPasswordRequest model)
            {
            _unitOfWork.ResetPassword(model);
                return Ok(new { message = "Password reset successful, you can now login" });
            }

            [Authorize(Role.Admin)]
            [HttpGet("GetAll")]
            public ActionResult<IEnumerable<AccountResponse>> GetAll()
            {
                var accounts = _unitOfWork.GetAll();
                return Ok(accounts);
            }

            [Authorize]
            [HttpGet("GetById")]
            public ActionResult<AccountResponse> GetById(string id)
            {
                var account = _unitOfWork.GetById(id);
                return Ok(account);
            }

            [Authorize(Role.Admin)]
            [HttpPost]
            public ActionResult<AccountResponse> CreateSubAdmin(CreateRequest model)
            {
                var account = _unitOfWork.Create(model);
                return Ok(account);
            }

            [Authorize]
            [HttpPut("Update")]
            public ActionResult<AccountResponse> Update(string id, UpdateRequest model)
            {
                var account = _unitOfWork.Update(id, model);
                return Ok(account);
            }

            [Authorize]
            [HttpDelete("Delete/{Id}")]
            public IActionResult Delete(string adminId, string userId)
            {
                // users can delete their own account and admins can delete any account
                if (adminId != ApplicationUser.Id && ApplicationUser.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

            _unitOfWork.Delete(userId);
                return Ok(new { message = "Account deleted successfully" });
            }
            [Authorize]
            [HttpPut("Archive/{Id}")]
            public async Task<IActionResult> Archive(string userId)
            {
            //// users can delete their own account and admins can delete any account
            if (userId != ApplicationUser.Id && ApplicationUser.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });


            await _unitOfWork.ArchiveUser(userId);
                return StatusCode(StatusCodes.Status200OK, "Account archived successfully" );
            }


            //Two Factor Authentication
            [Authorize]
            [HttpGet]
            [Route("SendUserOtp")]
            public async Task<IActionResult> SendUserOtp(string id)
            {
                try
                {
                    var isAuthenticated = User.Identity.IsAuthenticated;
                    var code = StringUtility.GenerateVerificationCode(6);
                    var account = await _db.ApplicationUsers.SingleOrDefaultAsync(x => x.Id == id);

                    // always return ok response to prevent email enumeration
                    if (account == null) return null;

                    // create reset token that expires after 1 day
                    account.OTP = code;
                    account.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);

                    _db.ApplicationUsers.Update(account);
                    _db.SaveChanges();
                    // SMS Service
                    var accountSid = ("ACe7643b8eb95e15efa182bffdfca15d15");
                    var authToken = ("dfab050cf9cbf3e5db5eae6842aa65d4");
                    var from = ("+14848044359");
                    var to = account.PhoneNumber;
                    TwilioClient.Init(accountSid, authToken);

                    var message = await MessageResource.CreateAsync
                        (
                        to: to,
                        from: from,
                        body: "Your medius security code is " + account.OTP
                        );
                    return StatusCode(StatusCodes.Status200OK, code);
                }
                catch (Exception ex)
                {
                    // Log exception code goes here
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            [Authorize]
            [HttpPost]
            [Route("AuthenticateUserOTP")]
            public async Task<IActionResult> AuthenticateUserOTP(string id, string OTP)
            {
                try
                {
                    var account = await _db.ApplicationUsers.SingleOrDefaultAsync(x => x.Id == id && x.OTP == OTP);

                    // always return ok response to prevent email enumeration
                    if (account == null) return null;

                    return StatusCode(StatusCodes.Status200OK, account);
                }
                catch (Exception ex)
                {
                    // Log exception code goes here
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
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
