using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Medius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _db;
        EmailSender _emailsender = new EmailSender();
        private const string LocalLoginProvider = "Local";
        private string emailRegex =
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        private string phoneRegex = @"^((\+|[0][0])[9][2](\d{3}))|((\d{4} ?)|(\d{4}-))?\d{7}$";
        public AccountController(ApplicationDbContext context, IUnitOfWork db)
        {
            _context = context;
            _db = db;
        }
        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = CreateResponseError("Could not changed password, invalid model",
                    ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)));
                return StatusCode(StatusCodes.Status400BadRequest, error);
            }

            //IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
            //    model.NewPassword);

            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}
            var success = CreateResponseError("",
                    ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)));
            return StatusCode(StatusCodes.Status200OK, success);
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = CreateResponseError("Could not set password, invalid model",
                      ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)));
                return StatusCode(StatusCodes.Status400BadRequest, error);
            }

            //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            var success = CreateResponseError("",
                    ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)));
            return StatusCode(StatusCodes.Status200OK, success);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (string.IsNullOrEmpty(loginViewModel.Email) || string.IsNullOrEmpty(loginViewModel.Password))
            {
                return BadRequest("Invalid Parameters.");
            }

            var user = _context.Users.FirstOrDefault(m =>
                m.Email == loginViewModel.Email || m.UserName == loginViewModel.Email);

            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }

            //if (UserManager.CheckPassword(user, loginViewModel.Password))
            //{
            //    var codeExists = _context.Users.Where(m =>
            //   m.ExpiryDate >= DateTime.Now && m.Email == loginViewModel.Email && m.Active == true).FirstOrDefault();

            //    if (codeExists == null)
            //    {
            //        return BadRequest("User is not verified or Code Expired");
            //    }
            //    var blob = GetUserForReturn(user);
            //    return Ok(blob);
            //}

            return BadRequest("Invalid Credentials");
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> Logout()
        {
            //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        //private IAuthenticationManager Authentication
        //{
        //    get { return Request.GetOwinContext(); }
        //}
        private JObject GetUserForReturn(ApplicationUser user, List<ApplicationUser> kids = null, bool register = false)
        {
            var tokenExpirationTimeSpan = TimeSpan.FromDays(14);
            ApplicationUser userGrades;
            if (user.City == null)
            {
                //userGrades = _context.Users.Include(m => m).FirstOrDefault(m => m.Id == user.Id);
            }
            else
            {
                userGrades = user;
            }
            // Sign-in the user using the OWIN flow
            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Name, user.UserName, null, "VerificationCode"));
            // This is very important as it will be used to populate the current user id 
            // that is retrieved with the User.Identity.GetUserId() method inside an API Controller
            identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id, null, "LOCAL_AUTHORITY"));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            var accesstoken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            //Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
            //Authentication.SignIn(identity);

            // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            

            JObject blob = new JObject(
                //new JProperty("Name", user.Name),
                //new JProperty("CreatedDate", user.),
                //new JProperty("access_token", accesstoken),
                //new JProperty("token_type", "bearer"),
                //new JProperty("expires_in", tokenExpirationTimeSpan.TotalSeconds.ToString()),
                //new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                //new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
                //new JProperty("LicenseExpiredDate", DateTime.Now.AddDays(30)),
                //new JProperty("LicenseActivationDate", DateTime.Now),
                //new JProperty("ApplicationUserId", user.Id),
                //new JProperty("Email", user.Email),
                //new JProperty("Avatar", user.Avatar),
                //new JProperty("GradeId", userGrades?.Grades?.FirstOrDefault()?.Id),
                //new JProperty("ProgressRank", studentStat.progressRank),
                //new JProperty("PerformanceRank", studentStat.performanceRank),
                //new JProperty("ParentApplicationUserId", user.ParentApplicationUserId),
                //new JProperty("Score", user.Score),
                //new JProperty("PaymentStatus", user.PaymentStatus),
                //new JProperty("AvailableUntil", user.AvailableUntil),
                //new JProperty("LastPaymentPlanId", user.LastPaymentPlanId),
                //new JProperty("LastPaymentDate", user.LastPaymentDate)
            );
            
            

            return blob;
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            var error = new ErrorViewModel();
            if (result == null)
            {
                error.Message = "Internal Server Error";
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach(string _error in result.Errors)
                    {
                        ModelState.AddModelError("", _error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }

                error.Message = "Invalid Request.";
                error.Error = ModelState.Values.SelectMany(m => m.Errors.Select(n => n.ErrorMessage));

                return StatusCode(StatusCodes.Status400BadRequest, error);
            }

            return null;
        }
        private ErrorViewModel CreateResponseError(string message, IEnumerable<string> errors)
        {
            var error = new ErrorViewModel
            {
                Message = message,
                Error = errors
            };
            return error;
        }
        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<System.Security.Claims.Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        //private static class RandomOAuthStateGenerator
        //{
        //    private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

        //    public static string Generate(int strengthInBits)
        //    {
        //        const int bitsPerByte = 8;

        //        if (strengthInBits % bitsPerByte != 0)
        //        {
        //            throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
        //        }

        //        int strengthInBytes = strengthInBits / bitsPerByte;

        //        byte[] data = new byte[strengthInBytes];
        //        _random.GetBytes(data);
        //        return HttpServerUtility.UrlTokenEncode(data);
        //    }
        //}

    }
}
