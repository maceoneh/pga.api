using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOs;
using pga.core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class Users : Base
    {
        public Users(Base b = null) : base(b)
        {
        }

        public async Task<bool> Add(DTOUser u)
        {
            var db_user = await this.DBLogic.ProxyStatement<DTOUser>();
            //Se comprueba si existe el usuario que se quiere dar de alta
            var exist_user = await db_user.FirstIfExistsAsync<DTOUser>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOUser.FilterUserMD5, ObjectValue = u.UserMD5, Type = FilterType.Equal }
                }
            });
            if (exist_user == null)
            {
                //Como no existe lo inserto en la base de datos
                if (await db_user.insertAsync(u))
                {
                    //Se genera el profile vacio
                    var db_user_profile = await this.DBLogic.ProxyStatement<DTOUserProfile>();
                    await db_user_profile.insertAsync(new DTOUserProfile
                    {
                        RefUser = u.ID,
                        UUID = Guid.NewGuid().ToString()
                    });
                    return true;
                }
                throw new Exception("Can't access to profile table");
            }
            else
            {
                throw new RegisterExistsException("User " + u.UserMD5 + " exists");
            }

            return false;
        }
    }
}
