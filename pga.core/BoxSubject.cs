﻿using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxSubject
    {
        private Box Box { get; }

        internal BoxSubject(Box b)
        {
            this.Box = b;
        }

        internal async Task<DTOBoxSubjectRoot> GetRoot()
        {
            var db_subjectroot = await this.Box.DBLogic.ProxyStatement<DTOBoxSubjectRoot>();
            var root = await db_subjectroot.FirstIfExistsAsync<DTOBoxSubjectRoot>();
            if (root == null)
            {
                //No existe usuario root, se crea uno por defecto
                var subject = await this.CreateSubject(new DTOBoxSubject { Name = "Root" });
                await this.AddSubjectTo(subject, EBoxSubjectType.Root);
                //var db_subject = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
                //await db_subject.insertAsync(new DTOBoxSubject {
                //    Name = "Root",
                //    UUID = Guid.NewGuid().ToString()
                //});
                //root = new DTOBoxSubjectRoot {
                //    RefSubject = db_subject.lastID,
                //    UserMD5 = MD5Utils.GetHash("admin"),
                //    PasswordMD5 = MD5Utils.GetHash("password")
                //};
                //await db_subjectroot.insertAsync(root);
            }
            return root;
        }

        internal async Task<bool> IsRoot(DTOBoxSubject s)
        {
            var actual_root = await this.GetRoot();
            return s.ID == actual_root.RefSubject;
        }


        /// <summary>
        /// Agrega a un sujeto una función especifica
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="o">Inicialización del objeto a crear si procede</param>
        /// <returns></returns>
        internal async Task<bool> AddSubjectTo(DTOBoxSubject s, EBoxSubjectType t, DTOBoxSubjectBaseRef o = null)
        {
            if (t == EBoxSubjectType.Root)
            {
                var db_subjectroot = await this.Box.DBLogic.ProxyStatement<DTOBoxSubjectRoot>();
                var root = await db_subjectroot.FirstIfExistsAsync<DTOBoxSubjectRoot>(new StatementOptions { 
                    Filters = new List<Filter> { 
                        new Filter { Name = DTOBoxSubjectRoot.FilterRefSubject, ObjectValue = s.ID, Type = FilterType.Equal }
                    }
                });
                if (root == null)
                {
                    if (o != null)
                    {
                        root = o as DTOBoxSubjectRoot;
                    }
                    if (root == null)
                    {
                        root = new DTOBoxSubjectRoot
                        {
                            RefSubject = s.ID,
                            UserMD5 = MD5Utils.GetHash(DTOBoxSubjectRoot.NoUsableUser),
                            PasswordMD5 = MD5Utils.GetHash(DTOBoxSubjectRoot.NoUsablePassword)
                        };
                    }
                    else
                    {
                        root.RefSubject = s.ID;
                    }
                    return await db_subjectroot.insertAsync(root);
                }
            }
            else if (t == EBoxSubjectType.Employ)
            { 
                var db_subjectemploy = await this.Box.DBLogic.ProxyStatement<DTOBoxSubjectEmploy>();
                var employ = await db_subjectemploy.FirstIfExistsAsync<DTOBoxSubjectEmploy>(new StatementOptions { 
                    Filters = new List<Filter> { 
                        new Filter { Name = DTOBoxSubjectEmploy.FilterRefSubject, ObjectValue = s.ID, Type = FilterType.Equal }
                    }
                });
                if (employ == null)
                {
                    if (o != null)
                    {
                        employ = o as DTOBoxSubjectEmploy;
                    }
                    if (employ == null)
                    {
                        employ = new DTOBoxSubjectEmploy
                        {
                            RefSubject = s.ID,
                            UserPGAMobile = ""
                        };
                    }
                    else
                    { 
                        employ.RefSubject = s.ID;
                    }
                    return await db_subjectemploy.insertAsync(employ);
                }
            }

            return false;
        }

        /// <summary>
        /// Crea un sujeto que no es ni usuario, ni root, ... solo se le permite ser asegurado o cliente
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<DTOBoxSubject> CreateSubject(DTOBoxSubject s)
        {
            var db_subjects = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
            s.UUID = Guid.NewGuid().ToString();
            await db_subjects.insertAsync(s);
            return s;
        }
    }
}