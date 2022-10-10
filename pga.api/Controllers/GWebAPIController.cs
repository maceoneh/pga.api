using es.dmoreno.utils.api;
using es.dmoreno.utils.security;
using es.dmoreno.utils.serialize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using pga.api.DTOs;
using pga.core;
using pga.core.DTOs;
using pga.core.DTOsBox;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

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
            var r = new DTOResponseGWebAPI<Object>
            {
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
            var parameters = (JsonElement)request.Parameters;
            switch (request.Action.ToUpper())
            {
                case "GETTOKEN":                    
                    var credentials = parameters.Deserialize<DTORequestGWebAPIGetToken>();
                    if (credentials != null)
                    {
                        r.Token = await this.GetToken(request.Provider, MD5Utils.GetHash(credentials.Username), MD5Utils.GetHash(credentials.Password));
                    }
                    break;
                //Funciones para el administrador
                case "CREATESESSION":
                    var infotoken = parameters.Deserialize<DTORequestGWebAPICreateSession>();
                    if (infotoken != null)
                    {
                        r.Response = await this.CreateSession(request.Provider, request.Token, infotoken);
                    }
                    else
                    {
                        r.Response = false;
                    }
                    break;
                case "REMOVESESSION":
                    var inforemovetoken = parameters.Deserialize<DTORequestGWebAPIRemoveSession>();
                    if (inforemovetoken != null)
                    {
                        r.Response = await this.RemoveSession(request.Provider, request.Token, inforemovetoken);
                    }
                    else
                    {
                        r.Response = false;
                    }
                    break;
                case "REFRESHTOKEN":
                    r.Response = true;
                    break;
                //Funciones para PGAMobile
                case "GETSTATUS":
                    var infoestatus = parameters.Deserialize<DTORequestGWebAPIGetStatus>();
                    if (infoestatus != null)
                    {
                        r.Response = new DTOResponseGWebAPIGetStatus
                        {
                            Identifier = await this.GetStatus(request.Provider, request.Token, infoestatus.DocumentType, infoestatus.IDRegistry)
                        };
                    }
                    else
                    {
                        r.Response = null;
                    }
                    break;
                case "SETWORKORDERSTATE":
                    var infoworkorderstate = parameters.Deserialize<DTORequestGWebAPIWorkOrderState>();
                    if (infoworkorderstate != null)
                    {
                        r.Response = await this.SetWorkOrderStatus(request.Provider, request.Token, infoworkorderstate);
                    }
                    else
                    {
                        r.Response = false;
                    }
                    break;
                case "SENDMESSAGE":
                    var infomessage = parameters.Deserialize<DTORequestGWebAPISendMessage>();
                    if (infomessage != null)
                    {
                        r.Response = await this.ReceiveMessage(request.Provider, request.Token, infomessage);
                    }
                    else
                    {
                        r.Response = false;
                    }
                    break;
                case "GETQUERY":
                    break;
                default:
                    throw new ArgumentException("Action '" + request.Action + "' not exists");
            }
            //Se devuelve la respuesta
            stopwatch.Stop();
            r.ExecutionTime = stopwatch.Elapsed.TotalSeconds;
            return r;
        }

        private async Task<bool> ReceiveMessage(string uuid_provider, string token, DTORequestGWebAPISendMessage msg)
        {
            using (var boxhelper = new Box(uuid_provider, token))
            {
                var filehelper = boxhelper.GetBoxFileHelper();
                var appointment = await filehelper.GetAppointmentByExternalIDAsync(msg.External_ID);
                if (appointment != null)
                {
                    if (!string.IsNullOrEmpty(msg.Value))
                    {
                        if (msg.EncodeType == EEncodeType.Base64)
                        {
                            msg.Value = Base64.Decode(msg.Value);
                        }
                    }
                    return await filehelper.AddMessageToAppointmentAsync(appointment, new DTOBoxMessage { 
                        Flow = EBoxMessageFlow.In,
                        Message = msg.Value,
                        Date = new DateTime(msg.TemporaryMark)
                    });

                }
            }
            return false;
        }

        private async Task<bool> SetWorkOrderStatus(string uuid_provider, string token, DTORequestGWebAPIWorkOrderState s)
        {
            using (var boxhelper = new Box(uuid_provider, token))
            {
                var filehelper = boxhelper.GetBoxFileHelper();
                var appointment = await filehelper.GetAppointmentByExternalIDAsync(s.IDMasterDetail);
                if (appointment != null) 
                {
                    return await filehelper.AddStatusDownloadedAsync(appointment);
                }
                else
                {
                    return false;
                }
            }
        }

        private async Task<string?> GetStatus(string uuid_provider, string token, int doctype, int id)
        {
            using (var boxhelper = new Box(uuid_provider, token))
            {
                return await boxhelper.GetBoxMessageHelper().GetLastIdentifierByDocumentAsync(EBoxDocumentType.Appointment, id);
            }
        }

        private async Task<string> GetToken(string uuid_provider, string username, string password)
        {
            using (var boxhelper = new Box(uuid_provider))
            {
                return await boxhelper.GetBoxSessionsHelper().GetTokenByUser(username, password);
            }
        }

        private async Task<bool> CreateSession(string uuid_provider, string token, DTORequestGWebAPICreateSession? info_newtoken)
        {
            if (info_newtoken != null)
            {
                using (var boxhelper = new Box(uuid_provider, token))
                {
                    return await boxhelper.GetBoxSessionsHelper().CreateSession(user_pgamobile: info_newtoken.User, appkey: info_newtoken.ApplicationKey, newtoken: info_newtoken.NewToken, ttl: info_newtoken.TTL, create_employ_if_not_exist: true);
                }
            }
            else
            {
                return false;
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
