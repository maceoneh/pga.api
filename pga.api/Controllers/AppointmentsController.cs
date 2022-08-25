using Microsoft.AspNetCore.Mvc;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("v1/appointments")]
    public class AppointmentsController : Controller
    {
        [HttpPost()]
        public async Task<string> CreateAppointment()
        {
            return null;
        }
    }
}
