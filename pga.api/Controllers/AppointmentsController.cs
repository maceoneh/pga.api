using Microsoft.AspNetCore.Mvc;
using pga.api.DTOs;
using pga.core;
using pga.core.DTOsBox;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("v1/appointments")]
    public class AppointmentsController : Controller
    {
        [HttpPost()]
        public async Task<string> CreateAppointment([FromBody] DTORequestCreateAppointment a)
        {
            using (var boxhelper = new Box("926d3a3d-09d3-4d68-a3e7-88432aadd7cb"))
            {                
                var subjecthelper = boxhelper.GetBoxSubjectHelper();
                //Se crea al receptor de la intervención
                a.Receiver = await subjecthelper.LoadOrCreateSubjectAsync(a.Receiver);
                //Se crea al proveedor si fuese necesario
                if (a.Provider != null)
                {
                    a.Provider = await subjecthelper.LoadOrCreateSubjectAsync(a.Provider);
                }
                //Se crea al intermediario si fuese necesario
                if (a.Intermediary != null)
                {
                    a.Intermediary = await subjecthelper.LoadOrCreateSubjectAsync(a.Intermediary);
                }
                //Se genera un expediente si fuese necesario
                var filehelper = boxhelper.GetBoxFileHelper();
                var f = await filehelper.CreateFile(new DTOBoxFile
                {
                    Receiver = a.Provider,
                    Provider = a.Provider,
                    Intermediary = a.Intermediary,
                    Policy = a.Policy,
                    Address = a.Address,
                    PostalCode = a.PostalCode,
                    Province = a.Province,
                    Population = a.Population,
                    Description = a.Description,
                    Date = a.DateFrom
                });

            }
            return null;
        }
    }
}
