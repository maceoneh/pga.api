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
        public async Task<bool> AddUser([FromBody] DTOUser user)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.Add(user);
            }
        }

        [HttpGet("users")]
        public async Task<List<DTOUser>> GFetList()
        {
            return new List<DTOUser> { 
                new DTOUser { UserMD5 = "aaaaaa", PasswordMD5 = "bbbbbbb" }
            };
        }
    }
}
