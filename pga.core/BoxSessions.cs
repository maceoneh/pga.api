using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using pga.core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxSessions
    {
        private Box Box { get; }    

        internal BoxSessions(Box b)
        {
            this.Box = b;
        }

        internal async Task<DTOBoxSubject> WhoIs()
        {
            var db_sessions = await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
            var session_to_check = await db_sessions.FirstIfExistsAsync<DTOBoxSession>(new StatementOptions { 
                Filters = new List<Filter> { 
                    new Filter { Name = DTOBoxSession.FilterToken, ObjectValue = this.Box.AccessToken, Type = FilterType.Equal }
                }
            });
            if (session_to_check != null)
            {
                //Esta dentro del TTL
                if (session_to_check.RefreshTime.AddMinutes(session_to_check.TTL) > DateTime.Now)
                {
                    //Se comprueba si pertenece a un usuario
                    var db_subjects = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
                    var subject = await db_subjects.FirstIfExistsAsync<DTOBoxSubject>(new StatementOptions
                    {
                        Filters = new List<Filter> {
                            new Filter { Name = DTOBoxSubject.FilterID, ObjectValue = session_to_check.RefSubject, Type = FilterType.Equal }
                        }
                    });
                    return subject;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetTokenByUser(string username, string password)
        {
            var subjecthelper = this.Box.GetBoxSubjectHelper();
            var root = await subjecthelper.GetRootAsync();            
            //El usuario es el root
            if (root.UserMD5 == username && root.PasswordMD5 == password)
            {
                //Se comprueba si existe otra sesion abierta (se programará según parametro de configuración, de momento un admin puede tener varias sesiones abiertas)
                //Se genera una nueva sesion
                var db_session = await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
                var new_session = new DTOBoxSession { 
                    Token = Token.generate(new ConfigToken { Length = 30, Letters = true, Numbers = true, UpperCase = false }),
                    CreationTime = DateTime.Now,
                    RefreshTime = DateTime.Now,
                    TTL = 60,
                    RefSubject = root.RefSubject
                };
                await db_session.insertAsync(new_session);
                return new_session.Token;
            }
            throw new UnauthorizedAccessException("User not exists");
        }

        /// <summary>
        /// Comprueba si el token indicando es válido ya que esta dentró del TTL y pertenece a un usuario
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> IsValidToken(string token)
        { 
            var db_sessions =  await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
            var session_to_check = await db_sessions.FirstIfExistsAsync<DTOBoxSession>(new StatementOptions { 
                Filters = new List<Filter> { 
                    new Filter { Name = DTOBoxSession.FilterToken, ObjectValue = token, Type = FilterType.Equal }
                }
            });
            if (session_to_check != null)
            {
                //Esta dentro del TTL
                if (session_to_check.RefreshTime.AddMinutes(session_to_check.TTL) > DateTime.Now)
                {
                    //Se comprueba si pertenece a un usuario
                    var db_subjects = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
                    var subject = await db_subjects.FirstIfExistsAsync<DTOBoxSubject>(new StatementOptions { 
                        Filters = new List<Filter> { 
                            new Filter { Name = DTOBoxSubject.FilterID, ObjectValue = session_to_check.RefSubject, Type = FilterType.Equal }
                        }
                    });
                    return subject != null;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Actualiza el campo refresh data a la fecha actual
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RefresToken(string token)
        {
            var db_sessions = await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
            var session_to_check = await db_sessions.FirstIfExistsAsync<DTOBoxSession>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxSession.FilterToken, ObjectValue = token, Type = FilterType.Equal }
                }
            });
            if (session_to_check != null)
            {
                session_to_check.RefreshTime = DateTime.Now;
                await db_sessions.updateAsync(session_to_check);
            }
        }

        public async Task<bool> RemoveSession(string appkey, string token)
        {
            var who_is = await this.Box.WhoIs();
            var boxsubjecthelper = this.Box.GetBoxSubjectHelper();
            if (await boxsubjecthelper.IsRootAsync(who_is))
            {
                var db_sessions = await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
                var session_to_delete = await db_sessions.FirstIfExistsAsync<DTOBoxSession>(new StatementOptions { 
                    Filters = new List<Filter> {
                        new Filter { Name = DTOBoxSession.FilterToken, ObjectValue = token, Type = FilterType.Equal },
                        new Filter { Name = DTOBoxSession.FilterApplicationKey, ObjectValue = appkey, Type = FilterType.Equal }
                    }
                });
                if (session_to_delete != null)
                {
                    return await db_sessions.deleteAsync(session_to_delete);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CreateSession(string user_pgamobile, string appkey, string newtoken, int ttl, bool create_employ_if_not_exist = false)
        {
            var who_is = await this.Box.WhoIs();
            var boxsubjecthelper = this.Box.GetBoxSubjectHelper();
            if (await boxsubjecthelper.IsRootAsync(who_is))
            {
                //Se busca el empleado
                var db_employee = await this.Box.DBLogic.ProxyStatement<DTOBoxSubjectEmploy>();
                var employ = await db_employee.FirstIfExistsAsync<DTOBoxSubjectEmploy>(new StatementOptions
                {
                    Filters = new List<Filter> { 
                        new Filter { Name = DTOBoxSubjectEmploy.FilterUserPGAMobile, ObjectValue = user_pgamobile, Type = FilterType.Equal }
                    }
                });
                //Si no existe se crea
                if (employ == null)
                {
                    if (create_employ_if_not_exist)
                    {
                        var new_subject = await boxsubjecthelper.CreateSubjectAsync(new DTOBoxSubject
                        {
                            Name = user_pgamobile,
                            eMail = user_pgamobile
                        });
                        employ = new DTOBoxSubjectEmploy { UserPGAMobile = user_pgamobile };
                        await boxsubjecthelper.AddSubjectToAsync(new_subject, EBoxSubjectType.Employ, employ);
                    }
                    else
                    {
                        throw new RegisterNotExistsException("Employ '" + user_pgamobile + "' not exists");
                    }
                }
                //Validar los datos de la sesion a crear
                //
                //Se genera la sesion
                var db_sessions = await this.Box.DBLogic.ProxyStatement<DTOBoxSession>();
                return await db_sessions.insertAsync(new DTOBoxSession { 
                    ApplicationKey = appkey,
                    CreationTime = DateTime.Now,
                    RefreshTime = DateTime.Now,
                    RefSubject = employ.RefSubject,
                    Token = newtoken,
                    TTL = ttl
                });
            }
            else
            {
                return false;
            }
        }
    }
}
