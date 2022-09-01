using Microsoft.AspNetCore.Mvc;
using pga.core;
using pga.core.DTOsBox;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("v1/files")]
    public class FilesController : Controller
    {
        [HttpPost()]
        public async Task<string> Create([FromBody] DTOBoxFile f)
        {
            using (var boxhelper = new Box(this.HttpContext.Items["_uuid_profile"].ToString()))
            {
                var fileshelper = boxhelper.GetBoxFileHelper();
                await fileshelper.CreateFile(f);
            }
            return f.UUID;
        }
    }
}
