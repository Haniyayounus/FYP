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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Medius.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseController
    {
            private readonly IApplicationUserRepository _unitOfWork;
            private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;


        public AccountController(IApplicationUserRepository unitOfWork, ApplicationDbContext db, IWebHostEnvironment env)
            {
                _unitOfWork = unitOfWork;
            _env = env;
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
            try
            {
                var account = _unitOfWork.Register(model, Request.Headers["origin"]);
                return StatusCode(StatusCodes.Status200OK, "Registration successful, please check your email for verification instructions");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, ex.Message);
            }
            }

        [HttpGet("VerifyEmail")]
        public ContentResult VerifyEmail(string token)
        {
            try
            {
                var account = _unitOfWork.VerifyEmail(token);

                if (account.Role == Role.SubAdmin)
                {
                    return base.Content("<img src='https://raw.githubusercontent.com/Haniyayounus/FYP/logo/circlecheck.png'"
                       + "style='width: 30%; height:50%; margin-left:35%;'/><p style='text-align:center; font - size:22px;'>Congratulations!</p>"
                       + "<p style = 'text-align: center; font-size:22px;'> You've successfully accepted the invitation.</p>"
                       + "<div class='modal-footer'>"
                       + "<a type='button' href='https://meet.google.com/szj-foaj-bek' style='background-color: #4CAF50; border: none; color: white; padding: 15px 32px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; margin-left: 45%'>"
                       + "Login Now</a><br /><br /></div>", "text/html");
                }
                else if (account.Role == Role.User)
                {
                    return base.Content("<img src='https://raw.githubusercontent.com/Haniyayounus/FYP/logo/circlecheck.png'"
                       + "style='width: 30%; height:50%; margin-left:35%;'/><p style='text-align:center; font - size:22px;'>Congratulations!</p>"
                       + "<p style = 'text-align: center; font-size:22px;'> You've successfully accepted the invitation.</p>"
                       + "<div class='modal-footer'>"
                       +"<br /><br /></div>", "text/html");
                }
                else
                {
                    return base.Content("Sorry something went wrong");
                }
            }
            catch(Exception ex)
            {
                return base.Content(ex.Message);
            }

            //File("~/Images/photo.jpg", "image/jpeg");
            //return StatusCode(StatusCodes.Status200OK, );
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
                var authToken = ("");
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
            
            [HttpGet("ValidateResetToken")]
            public IActionResult ValidateResetToken(string resetToken)
            {
            try
            {
                _unitOfWork.ValidateResetToken(resetToken);
                return StatusCode(StatusCodes.Status200OK, "Token is valid" );
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            }

            [HttpPost("ResetPassword")]
            public IActionResult ResetPassword(ResetPasswordRequest model)
            {
            try
            {
                _unitOfWork.ResetPassword(model);
                return StatusCode(StatusCodes.Status200OK, "Password reset successful, you can now login" );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, ex.Message);
            }
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

            [Authorize]
            [HttpPut("Update")]
            public ActionResult<AccountResponse> Update(string id, UpdateRequest model)
            {
            try
            {
                var account = _unitOfWork.Update(id, model);
                return Ok(account);
            }
                catch(Exception ex)
            {
                return Ok(ex.Message);
            }
            }

            [Authorize]
            [HttpDelete("Delete")]
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
                    var account = _unitOfWork.GetUser(id);

                    // always return ok response to prevent email enumeration
                    if (account == null) return null;

                
                    // create reset token that expires after 1 day
                    //account.OTP = code;
                    //account.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);

                _unitOfWork.UpdateOTP(id, code);
                    //_db.SaveChanges();
                    // SMS Service
                    var accountSid = ("ACe7643b8eb95e15efa182bffdfca15d15");
                    var authToken = ("");
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
            [HttpGet]
            [Route("AuthenticateUserOTP")]
            public async Task<IActionResult> AuthenticateUserOTP(string id, string OTP)
            {
                try
                {
                var account = _unitOfWork.GetUser(id);
                if(account.OTP != OTP)
                    return StatusCode(StatusCodes.Status404NotFound, "Incorrect OTP");

                    return StatusCode(StatusCodes.Status200OK, "OTP Verified");
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
