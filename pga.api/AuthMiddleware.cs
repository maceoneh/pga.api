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
        }

        protected async override Task<bool> checkValidationAsync(HttpContext context)
        {
            var auth_type = context.Request.Headers["_auth_type"];
            try
            {
                switch (auth_type)
                {
                    case "basic":
                        return await this.checkBasic(context);
                    case "bearer":
                        return await this.checkBearer(context);
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<bool> checkBasic(HttpContext context)
        {
            var path = context.Request.Path;
            if (path.Equals("/administration/users") || path.Equals("/administration/users/"))
            {
                var mdhelper = new MasterData();
                //, context.Request.Headers["_password"]
                var admin_user = await mdhelper.GetUserAdminMD5();
                var admin_password = await mdhelper.GetPasswordAdminMD5();
                return admin_user.Equals(context.Request.Headers["_user"]) && admin_password.Equals(context.Request.Headers["_password"]);
            }
            return false;
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
