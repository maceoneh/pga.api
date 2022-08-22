using es.dmoreno.utils.api;
using es.dmoreno.utils.security;
using es.dmoreno.utils.serialize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pga.api.DTOs;
using pga.core;
using pga.core.DTOs;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace pga.api.Controllers
{
    [ApiController]
    [Route("gwebapi")]
    public class GWebAPIController : Controller
    {
        private static readonly string[] ActionWithoutTokenValidation = new string[] { 
            "GETTOKEN"
        };

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
            //Se refresca el token si procede
            if (ActionWithoutTokenValidation.Where(reg => reg == request.Action.ToUpper()).Count() == 0)
            {
                using (var boxhelper = new Box(request.Provider))
                {
                    var sessionshelper = boxhelper.GetBoxSessionsHelper();
                    if (await sessionshelper.IsValidToken(request.Token))
                    {
                        await sessionshelper.RefresToken(request.Token);
                    }
                    else
                    {
                        throw new PGAAPIException { StatusCode = StatusCodes.Status403Forbidden, StatusMessage = "Not valid token" };
                    }
                }
            }
            //Se comprueba la accion
            switch (request.Action.ToUpper())
            {
                case "GETTOKEN":                    
                    var credentials = JSon.JObjectToType<DTORequestGWebAPIGetToken>(request.Parameters as JObject);
                    response.Token = await this.GetToken(request.Provider, MD5Utils.GetHash(credentials.Username), MD5Utils.GetHash(credentials.Password));                    
                    break;
                case "CREATESESSION":
                    var infotoken = JSon.JObjectToType<DTORequestGWebAPICreateSession>(request.Parameters as JObject);
                    response.Response = await this.CreateSession(request.Provider, request.Token, infotoken);
                    break;
                case "REMOVESESSION":
                    var inforemovetoken = JSon.JObjectToType<DTORequestGWebAPIRemoveSession>(request.Parameters as JObject);
                    response.Response = await this.RemoveSession(request.Provider, request.Token, inforemovetoken);
                    break;
                default:
                    throw new ArgumentException("Action '" + request.Action + "' not exists");
            }
            //Se devuelve la respuesta
            stopwatch.Stop();
            response.ExecutionTime = stopwatch.Elapsed.TotalSeconds;
            return response;
        }

        private async Task<string> GetToken(string uuid_provider, string username, string password)
        {
            using (var boxhelper = new Box(uuid_provider))
            {
                return await boxhelper.GetBoxSessionsHelper().GetTokenByUser(username, password);
            }
        }

        private async Task<bool> CreateSession(string uuid_provider, string token, DTORequestGWebAPICreateSession info_newtoken)
        {
            using (var boxhelper = new Box(uuid_provider, token))
            {
                return await boxhelper.GetBoxSessionsHelper().CreateSession(user_pgamobile: info_newtoken.User, appkey: info_newtoken.ApplicationKey, newtoken: info_newtoken.NewToken, ttl: info_newtoken.TTL, create_employ_if_not_exist: true);
            }
        }

        private async Task<bool> RemoveSession(string uuid_provider, string token, DTORequestGWebAPIRemoveSession info_oldtoken)
        {
            using (var boxhelper = new Box(uuid_provider, token))
            {
                return await boxhelper.GetBoxSessionsHelper().RemoveSession(info_oldtoken.ApplicationKey, info_oldtoken.OldToken);
            }
        }
    }
}
