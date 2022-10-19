using Microsoft.AspNetCore.Mvc;
using pga.api.DTOs;
using pga.core;
using pga.core.DTOsBox;
using System.Net;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("v1/appointments")]
    public class AppointmentsController : Controller
    {
        [HttpPost()]
        public async Task<string?> CreateAppointment([FromBody] DTORequestCreateAppointment a)
        {
            using (var boxhelper = new Box(this.HttpContext.Items["_uuid_profile"].ToString()))
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
                    Receiver = a.Receiver,
                    Provider = a.Provider,
                    Intermediary = a.Intermediary,
                    Policy = a.Policy,
                    Address = a.Address,
                    PostalCode = a.PostalCode,
                    Province = a.Province,
                    Population = a.Population,
                    Description = a.Description,
                    Date = a.DateFrom,
                    ExternalID = a.ExternalIDFile,
                });
                //Se busca al empleado y si no existe se crea
                var employ = await subjecthelper.GetEmployByPGAMobileUser(a.PGAMobileUser);
                if (employ == null)
                {
                    //Si el usuario no existe se da de alta
                    employ = new DTOBoxSubject { 
                        Name = a.PGAMobileUser                        
                    };
                    employ = await subjecthelper.CreateSubjectAsync(employ);
                    if (!await subjecthelper.AddSubjectToAsync(employ, EBoxSubjectType.Employ, new DTOBoxSubjectEmploy { UserPGAMobile = a.PGAMobileUser }))
                    {
                        throw new PGAAPIException { StatusCode = (int)HttpStatusCode.BadRequest, StatusMessage = "Unable to create user " + a.PGAMobileUser };
                    }
                }
                var employees = new List<DTOBoxEmployInAppointment> { new DTOBoxEmployInAppointment { 
                    Employ = employ,
                    Leading = true
                } };
                //Se crea una cita en el expediente
                var new_appointment = await filehelper.AddAppointmentAsync(new DTOBoxAppointment
                {
                    Agreed = false,
                    DateFrom = a.DateFrom,
                    DateTo = a.DateFrom.AddHours(1),
                    Description = a.Description,
                    GuildDescription = a.Guild,
                    ExternalID = a.ExternalID,
                    EmployeesInAppointment = employees
                }, f);
                if (new_appointment != null)
                {
                    return new_appointment.UUID;
                }
                
            }
            return null;
        }
    }
}
