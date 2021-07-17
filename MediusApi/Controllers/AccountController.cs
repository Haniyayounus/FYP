using Medius.DataAccess.Repository.IRepository;
using MediusApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MediusApi.Controllers
{
    [System.Web.Http.Route("api/Account")]
    [ApiController]
    public class BaseApiController : ApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _db;
        EmailSender _emailSender = new EmailSender();
        private const string LocalLoginProvider = "Local";
    }
}
