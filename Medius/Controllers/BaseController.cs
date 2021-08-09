using Medius.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medius.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public ApplicationUser ApplicationUser => (ApplicationUser)HttpContext.Items["ApplicationUser"];
    }
}
