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
            using (var boxhelper = new Box("926d3a3d-09d3-4d68-a3e7-88432aadd7cb"))
            {
                var fileshelper = boxhelper.GetBoxFileHelper();
                await fileshelper.CreateFile(f);
            }
            return f.UUID;
        }
    }
}
