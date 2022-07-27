using es.dmoreno.utils.debug;
using es.dmoreno.utils.serialize;
using Microsoft.AspNetCore.Mvc.Filters;
using pga.api.DTOs;
using pga.core.Exceptions;
using System.Text;

namespace pga.api
{
    public class FilterException : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            DTOResponseGWebAPI<DTOError> e;
            string dataJson;
            byte[] data;

            e = new DTOResponseGWebAPI<DTOError>()
            {
                Response = null,
                Error = new DTOError()
            };

            if (context.Exception is PGAAPIException)
            {
                e.Error.Code = (context.Exception as PGAAPIException).StatusCode;
                e.Error.Message = (context.Exception as PGAAPIException).StatusMessage;
            }
            if (context.Exception is RegisterExistsException)
            {
                e.Error.Code = StatusCodes.Status406NotAcceptable;
                e.Error.Message = (context.Exception as RegisterExistsException).Message;
            }
            else
            {
                e.Error.Code = StatusCodes.Status500InternalServerError;
                e.Error.Message = context.Exception.Message;
            }

            if (e.Error.Message == null)
            {
                e.Error.Message = context.Exception.Message;
            }

            context.HttpContext.Response.StatusCode = e.Error.Code;

            dataJson = JSon.serializeJSON<DTOResponseGWebAPI<DTOError>>(e);

            data = Encoding.UTF8.GetBytes(dataJson);
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);

            context.ExceptionHandled = true;

            Log.Write(ETypeLog.Error, context.Exception.GetType().Name, e.Error.Message + Environment.NewLine + context.Exception.StackTrace);
        }
    }
}
