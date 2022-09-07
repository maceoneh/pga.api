using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.dataaccess.textplain;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxActivity
    {
        private Box Box { get; }

        internal BoxActivity(Box b)
        {
            this.Box = b;
        }

        /// <summary>
        /// Inserta una actividad de la aplicación
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        internal async Task<bool> AddAsync(DTOBoxActivity a)
        {
            string? aux = null;
            a.Date = DateTime.Now;
            a.Identifier = Token.generate(new ConfigToken { Length = 20, Letters = true, Numbers = true });
            if (a.Activity != null)
            {
                if (a.Activity.Length > 2500)
                {
                    aux = a.Activity;
                    a.Activity = null;

                }
            }
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxActivity>();
            if (await db_activity.insertAsync(a))
            {
                if (aux != null)
                {
                    a.File = a.ID.ToString() + ".json";
                    using (var filetexthelper = new TextPlainFile(this.Box.ActivityPath + @"\" + a.File))
                    {
                        filetexthelper.set(aux);
                    }
                    await db_activity.updateAsync(a);
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Obtiene a raiz de un identificador los siguientes registros de actividad relacionados con el identificvador indicado
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<List<DTOBoxActivity>?> GetListFromIdentifierAsync(string identifier, EBoxDocumentType type)
        {
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxActivity>();
            var first_activity = await db_activity.FirstIfExistsAsync<DTOBoxActivity>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxActivity.FilterIdentifier, ObjectValue = identifier, Type = FilterType.Equal }
                }
            });
            if (first_activity == null)
            {
                return null;
            }
            else
            {
                var filters = new List<Filter> {
                    new Filter { Name = DTOBoxActivity.FilterID, ObjectValue = first_activity.ID, Type = FilterType.Greater }
                };

                if (type == EBoxDocumentType.File)
                {
                    if (first_activity.RefFile > 0)
                    {
                        filters.Add(new Filter { Name = DTOBoxActivity.FilterRefFile, ObjectValue = first_activity.RefFile, Type = FilterType.Equal });
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (type == EBoxDocumentType.Appointment)
                {
                    if (first_activity.RefAppointment > 0)
                    {
                        filters.Add(new Filter { Name = DTOBoxActivity.FilterRefAppointment, ObjectValue = first_activity.RefFile, Type = FilterType.Equal });
                    }
                    else
                    {
                        return null;
                    }
                }

                var list = new List<DTOBoxActivity>();
                list.Add(first_activity);
                list.AddRange(await db_activity.selectAsync<DTOBoxActivity>(new StatementOptions { Filters = filters }));
                return list.OrderBy(reg => reg.Date).ToList();
            }
        }

        public async Task<string?> GetLastIdentifierByDocumentAsync(EBoxDocumentType type, int idregistry)
        {
            string? filter;
            switch (type)
            {
                case EBoxDocumentType.Appointment:
                    filter = DTOBoxActivity.FilterRefAppointment;
                    break;
                case EBoxDocumentType.File:
                    filter = DTOBoxActivity.FilterRefFile;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxActivity>();
            var first_activity = await db_activity.FirstIfExistsAsync<DTOBoxActivity>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = filter, ObjectValue = idregistry, Type = FilterType.Equal }
                },
                OrderBy = "id DESC"
            });
            if (first_activity != null)
            {
                return first_activity.Identifier;
            }
            else
            {
                return null;
            }
        }
    }
}
