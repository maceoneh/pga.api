using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOs;
using pga.core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class Users : Base
    {
        public Users(Base b = null) : base(b)
        {
        }

        public async Task<string> Add(DTOUser u)
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
                    var new_profile = new DTOUserProfile
                    {
                        RefUser = u.ID,
                        UUID = Guid.NewGuid().ToString()
                    };
                    await db_user_profile.insertAsync(new_profile);
                    return new_profile.UUID;
                }
                throw new Exception("Can't access to profile table");
            }
            else
            {
                throw new RegisterExistsException("User " + u.UserMD5 + " exists");
            }
        }

        public async Task<List<DTOUserProfile>> GetProfiles()
        {
            var db_profiles = await this.DBLogic.ProxyStatement<DTOUserProfile>();
            return await db_profiles.selectAsync<DTOUserProfile>();
        }

        /// <summary>
        /// Obtiene los datos de un profile a partir de su usuario y contraseña
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<DTOUserProfile?> GetProfileAsync(string user, string password)
        {
            var db_users = await this.DBLogic.ProxyStatement<DTOUser>();
            var user_in_db = await db_users.FirstIfExistsAsync<DTOUser>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOUser.FilterUserMD5, ObjectValue = user, Type = FilterType.Equal }
                }
            });
            if (user_in_db == null)
            {
                return null;
            }
            else
            {
                if (user_in_db.PasswordMD5 == password)
                {
                    var db_profiles = await this.DBLogic.ProxyStatement<DTOUserProfile>();
                    return await db_profiles.FirstIfExistsAsync<DTOUserProfile>(new StatementOptions
                    {
                        Filters = new List<Filter> {
                            new Filter { Name = DTOUserProfile.FilterRefUser, ObjectValue = user_in_db.ID, Type = FilterType.Equal }
                        }
                    });
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<DTOUserProfile> GetProfile(string uuid)
        {
            var db_profiles = await this.DBLogic.ProxyStatement<DTOUserProfile>();
            var profile_in_db = await db_profiles.FirstIfExistsAsync<DTOUserProfile>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOUserProfile.FilterUUID, ObjectValue = uuid, Type = FilterType.Equal }
                }
            });
            if (profile_in_db == null)
            {
                throw new KeyNotFoundException("User with UUID " + uuid + " not found");
            }
            else
            {
                return profile_in_db;
            }
        }

        public async Task<bool> SetProfile(DTOUserProfile p)
        {
            //Se busca el profile en la BD
            var db_profiles = await this.DBLogic.ProxyStatement<DTOUserProfile>();
            var profile_in_db = await db_profiles.FirstIfExistsAsync<DTOUserProfile>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOUserProfile.FilterUUID, ObjectValue = p.UUID, Type = FilterType.Equal }
                }
            });
            if (profile_in_db == null)
            {
                throw new KeyNotFoundException("User with UUID " + p.UUID + " not found");
            }
            else
            {
                //Se actualizan los datos
                profile_in_db.Name = p.Name;
                profile_in_db.Surname = p.Surname;
                profile_in_db.Address = p.Address;
                profile_in_db.PostalCode = p.PostalCode;
                profile_in_db.Province = p.Province;
                profile_in_db.Population = p.Population;
                profile_in_db.eMail = p.eMail;
                return await db_profiles.updateAsync(profile_in_db);
            }
        }

        public async Task<bool> DeleteProfile(string uuid)
        {
            var db_profiles = await this.DBLogic.ProxyStatement<DTOUserProfile>();
            var profile_in_db = await db_profiles.FirstIfExistsAsync<DTOUserProfile>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOUserProfile.FilterUUID, ObjectValue = uuid, Type = FilterType.Equal }
                }
            });
            if (profile_in_db == null)
            {
                throw new KeyNotFoundException("User with UUID " + uuid + " not found");
            }
            else
            {
                //Se obtiene el usuario asociado
                var db_users = await this.DBLogic.ProxyStatement<DTOUser>();
                var user_in_db = await db_users.FirstIfExistsAsync<DTOUser>(new StatementOptions
                {
                    Filters = new List<Filter> {
                        new Filter { Name = DTOUser.FilterID, ObjectValue = profile_in_db.RefUser, Type = FilterType.Equal }
                    }
                });
                var transaction_id = new object();
                try
                {
                    db_profiles.beginTransaction(transaction_id);
                    if (user_in_db != null)
                    {
                        db_users.beginTransaction(transaction_id);
                        await db_users.deleteAsync(user_in_db);
                    }
                    await db_profiles.deleteAsync(profile_in_db);
                    if (user_in_db != null)
                    {
                        db_users.acceptTransaction(transaction_id);
                    }
                    db_profiles.acceptTransaction(transaction_id);
                }
                catch
                {
                    if (user_in_db != null)
                    {
                        db_users.refuseTransaction(transaction_id);
                    }
                    db_profiles.refuseTransaction(transaction_id);
                }
                return true;
            }
        }
    }
}
