using Microsoft.AspNetCore.Mvc;
using pga.core;
using pga.core.DTOs;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("administration")]
    public class AdministrationController : Controller
    {
        [HttpPost("users")]
        public async Task<bool> AddUser(DTOUser user)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.Add(user);
            }
        }
    }
}
