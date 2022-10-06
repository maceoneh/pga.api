using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.dataaccess.textplain;
using es.dmoreno.utils.permissions;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxMessage
    {
        /// <summary>
        /// Indica el temaño máximo que un mensaje debe tener para ser almacenado en la base de datos.
        /// El tamaño está calculado para almacenar mensajes cortos de texto en la base de datos.
        /// </summary>
        public const int MaxValueLength = 255;

        private Box Box { get; }

        internal BoxMessage(Box b)
        {
            this.Box = b;
        }

        /// <summary>
        /// Inserta una actividad de la aplicación
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        internal async Task<bool> AddAsync(DTOBoxMessage a, bool fill_date = true)
        {
            string? aux = null;
            if (fill_date || a.Date == null)
            {
                a.Date = DateTime.Now;
            }
            a.Identifier = Token.generate(new ConfigToken { Length = 20, Letters = true, Numbers = true });
            if (a.Message != null)
            {
                if (a.Message.Length > MaxValueLength)
                {
                    aux = a.Message;
                    a.Message = null;

                }
            }
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxMessage>();
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
                //Se agregan los permisos al registro
                var users_group = await this.Box.GetUsersGroupAsync();
                DTOUUIDRecordPermision? record_permissions = null;
                if (users_group != null)
                {
                    record_permissions = new DTOUUIDRecordPermision { 
                        UUID = users_group.UUID,
                        CanRead = true,
                        CanWrite = false
                    };                    
                }
                using (var permissionshelper = new Permissions(this.Box.DataPath))
                {
                    await permissionshelper.AddPermissionAsync<DTOBoxMessage>(a,
                        new DTORecordPermission
                        {
                            UUIDOwner = (await this.Box.WhoIs()).UUID,
                            UUIDRecordPermissions = new DTOUUIDRecordPermision[] {
                                record_permissions
                            }
                        });
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
        public async Task<List<DTOBoxMessage>?> GetListFromIdentifierAsync(string identifier, EBoxDocumentType type)
        {
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxMessage>();
            var first_activity = await db_activity.FirstIfExistsAsync<DTOBoxMessage>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxMessage.FilterIdentifier, ObjectValue = identifier, Type = FilterType.Equal }
                }
            });
            if (first_activity == null)
            {
                return null;
            }
            else
            {
                var filters = new List<Filter> {
                    new Filter { Name = DTOBoxMessage.FilterID, ObjectValue = first_activity.ID, Type = FilterType.Greater }
                };

                if (type == EBoxDocumentType.File)
                {
                    if (first_activity.RefFile > 0)
                    {
                        filters.Add(new Filter { Name = DTOBoxMessage.FilterRefFile, ObjectValue = first_activity.RefFile, Type = FilterType.Equal });
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
                        filters.Add(new Filter { Name = DTOBoxMessage.FilterRefAppointment, ObjectValue = first_activity.RefFile, Type = FilterType.Equal });
                    }
                    else
                    {
                        return null;
                    }
                }

                var list = new List<DTOBoxMessage>();
                list.Add(first_activity);
                list.AddRange(await db_activity.selectAsync<DTOBoxMessage>(new StatementOptions { Filters = filters }));
                return list.OrderBy(reg => reg.Date).ToList();
            }
        }

        public async Task<string?> GetLastIdentifierByDocumentAsync(EBoxDocumentType type, int idregistry)
        {
            string? filter;
            switch (type)
            {
                case EBoxDocumentType.Appointment:
                    filter = DTOBoxMessage.FilterRefAppointment;
                    break;
                case EBoxDocumentType.File:
                    filter = DTOBoxMessage.FilterRefFile;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var db_activity = await this.Box.DBLogic.ProxyStatement<DTOBoxMessage>();
            var first_activity = await db_activity.FirstIfExistsAsync<DTOBoxMessage>(new StatementOptions
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
