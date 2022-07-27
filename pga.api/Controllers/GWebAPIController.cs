using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pga.api.DTOs;
using System.Text.Json.Nodes;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("gwebapi")]
    public class GWebAPIController : Controller
    {
        [HttpPost()]
        public async Task<Object> ProcessPost([FromBody] DTORequestGWebAPI<Object> o)
        {
            var r = new DTOResponseGWebAPI<bool> { 
                Response = true
            };

            return r;
        }

        [HttpGet("sample_error")]
        public async Task SampleException()
        {
            throw new Exception("Sample exception");
        }
    }
}
