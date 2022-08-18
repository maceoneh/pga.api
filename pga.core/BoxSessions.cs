﻿using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<string> GetTokenByUser(string username, string password)
        {
            var subjecthelper = this.Box.GetBoxSubjectHelper();
            var root = await subjecthelper.GetRoot();
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
    }
}
