using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Medius.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        //[HttpPost]
        //[AllowAnonymous]
        //[Route("Login")]
        //public IHttpActionResult Login(LoginViewModel loginViewModel)
        //{
        //    if (string.IsNullOrEmpty(loginViewModel.Email) || string.IsNullOrEmpty(loginViewModel.Password))
        //    {
        //        return BadRequest("Invalid Parameters.");
        //    }

        //    var user = _context.Users.FirstOrDefault(m =>
        //        m.Email == loginViewModel.Email || m.UserName == loginViewModel.Email);

        //    if (user == null)
        //    {
        //        return BadRequest("Invalid Credentials");
        //    }

        //    if (UserManager.CheckPassword(user, loginViewModel.Password))
        //    {
        //        var codeExists = _context.VerificationCodeHistories.Where(m =>
        //       m.ExpiryDate >= DateTime.Now && m.Destination == loginViewModel.Email && m.Active == true).FirstOrDefault();

        //        if (codeExists == null)
        //        {
        //            return BadRequest("User is not verified or Code Expired");
        //        }
        //        var blob = GetUserForReturn(user);
        //        return Ok(blob);
        //    }

        //    return BadRequest("Invalid Credentials");
        //}
    }
}
