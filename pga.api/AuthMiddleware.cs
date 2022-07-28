using es.dmoreno.utils.corenet.api.middleware;
using pga.core;

namespace pga.api
{
    public class AuthMiddleware : AuthorizationMiddlewareBase
    {
        public AuthMiddleware(RequestDelegate next) : base(next)
        {
            if (AuthMiddleware.ResourcesWithoutAuthorization.Count() == 0)
            {
                AuthMiddleware.ResourcesWithoutAuthorization = new string[] {
                    //"regex",
                    //"/api/licenses/user$",
                    //"POST"
                };
            }

            if (AuthMiddleware.ResourcesByTypeWithAuthorization.Count() == 0)
            {
                AuthMiddleware.ResourcesByTypeWithAuthorization = new string[] { 
                    //Forma de realizar checkeo (equal)
                    //Recurso
                    //Metodo
                    //Tipo de autorizacion
                    "equal",
                    "/administration/users",
                    "POST",
                    "BASIC_1",

                    "equal",
                    "/administration/users/",
                    "POST",
                    "BASIC_1",

                    "equal",
                    "/administration/profiles",
                    "GET",
                    "BASIC_1",

                    "equal",
                    "/administration/profiles/",
                    "GET",
                    "BASIC_1"
                };
            }
        }

        protected async override Task<bool> checkValidationAsync(HttpContext context)
        {
            var auth_type = GetAuthorizationType(context);
            switch (auth_type)
            {
                case "BASIC_1":
                    return await this.checkBasic1(context);
                default:
                    return false;
            }
        }

        private async Task<bool> checkBasic1(HttpContext context)
        {
            var mdhelper = new MasterData();
            var admin_user = await mdhelper.GetUserAdminMD5();
            var admin_password = await mdhelper.GetPasswordAdminMD5();
            return admin_user.Equals(context.Request.Headers["_user"]) && admin_password.Equals(context.Request.Headers["_password"]);
        }

        private async Task<bool> checkBearer(HttpContext context)
        {
            //var path = context.Request.Path;
            //switch (path)
            //{
            //    case "/api/licenses/session/data":
            //    case "/api/licenses/user/profile":
            //    case "/api/licenses/session/environment":
            //        using (var shelper = Core.Create().BuildSessionHelper())
            //        {
            //            var uuid = await shelper.GetUserUUID(context.Request.Headers["_token"]);
            //            context.Request.Headers.Add("_uuid_user", uuid);
            //            var session = await shelper.GetSessionByIdentifier(context.Request.Headers["_token"]);
            //            if (session.IsExpired)
            //            {
            //                return false;
            //            }
            //            context.Items["session_data"] = session;
            //        }
            //        return true;
            //}
            return false;
        }
    }
}
