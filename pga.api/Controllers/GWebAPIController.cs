using es.dmoreno.utils.api;
using es.dmoreno.utils.security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pga.api.DTOs;
using pga.core;
using pga.core.DTOs;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            return await this.Process(o);                        
        }

        [HttpGet("sample_error")]
        public async Task SampleException()
        {
            throw new Exception("Sample exception");
        }

        private async Task<object> Process(DTORequestGWebAPI<Object> request)
        {
            //Se inicia un cronometro
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //Se comprueba que el proveedor exista
            using (var usershelper = new Users())
            {
                var profile = await usershelper.GetProfile(request.Provider);
            }
            //Se comprueba si la base de datos esta actualizada
            using (var boxhelper = new Box(request.Provider))
            {
                await boxhelper.CreateUpdateDatabase();                
            }
            //Se genera paquete de respuesta
            var response = new DTOResponseGWebAPI<Object> { 
                AKType = "response",
                Debug = false,
                Error = null,
                Provider = request.Provider,
                Request = null,
                Token = request.Token,
                Version = 1
            };
            //Se comprueba la accion
            switch (request.Action.ToUpper())
            {
                case "GETTOKEN":
                    response.Token = await this.GetToken();                    
                    break;
                default:
                    throw new ArgumentException("Action '" + request.Action + "' not exists");
            }
            //Se devuelve la respuesta
            stopwatch.Stop();
            response.ExecutionTime = stopwatch.Elapsed.TotalSeconds;
            return response;
        }

        private async Task<string> GetToken()
        {
            return "";           
        }
    }
}
