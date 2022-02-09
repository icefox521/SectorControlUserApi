using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SectorControlApi.Constants;
using System.Security.Claims;

namespace SectorControlApi.Controllers
{
    /// <summary>
    /// Controller for generic methods to help during development and debugging.
    /// </summary>
    [Route("/[action]")]
    public class CommonController : Controller
    {
        [HttpGet]
        [ActionName("echo")]
        public string Echo()
        {
            return "api for Sector Control";
        }

        [Authorize(AuthenticationSchemes = "UserAuthentication")]
        [HttpGet]
        [ActionName("whoami")]
        public string WhoAmI()
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            var userId = identity?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
            return "user id: " + userId;
        }
    }
}
