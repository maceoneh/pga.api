using es.dmoreno.utils.security;
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
        public async Task<string> AddUser([FromBody] DTOUser user)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.Add(user);
            }
        }

        [HttpGet("profiles")]
        public async Task<List<DTOUserProfile>> GetProfileList()
        {
            using (var uhelper = new Users())
            {
                return await uhelper.GetProfiles();
            }
        }

        [HttpGet("profiles/{uuid_base64}")]
        public async Task<DTOUserProfile> GetProfile(string uuid_base64)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.GetProfile(Base64.Decode(uuid_base64));
            }
        }

        [HttpDelete("profiles/{uuid_base64}")]
        public async Task<bool> DeleteProfile(string uuid_base64)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.DeleteProfile(Base64.Decode(uuid_base64));
            }
        }

        [HttpPut("profiles")]
        public async Task<bool> UpdateProfile([FromBody] DTOUserProfile p)
        {
            using (var uhelper = new Users())
            {
                return await uhelper.SetProfile(p);
            }
        }
    }
}
