using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace pga.core
{
    public class BoxGuild
    {
        private Box Box { get; }

        internal BoxGuild(Box b)
        { 
            this.Box = b;
        }

        /// <summary>
        /// Agrega un gremio al listado
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public async Task<DTOBoxGuild> AddSync(string name)
        {
            name = name.Trim().ToUpper();
            var db_guilds = await this.Box.DBLogic.ProxyStatement<DTOBoxGuild>();
            var guild = await db_guilds.FirstIfExistsAsync<DTOBoxGuild>(new StatementOptions { 
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxGuild.FilterName, ObjectValue = name, Type = FilterType.Equal }
                }
            });
            if (guild == null)
            {
                guild = new DTOBoxGuild { Name = name };
                await db_guilds.insertAsync(guild);
            }
            return guild;
        }

        /// <summary>
        /// Obtiene el listado de gremios
        /// </summary>
        /// <returns></returns>
        public async Task<List<DTOBoxGuild>> GetListAsync()
        {
            var db_guilds = await this.Box.DBLogic.ProxyStatement<DTOBoxGuild>();
            return await db_guilds.selectAsync<DTOBoxGuild>();
        }

        internal async Task<DTOBoxGuild> GetByID(int id)
        {
            var db_guilds = await this.Box.DBLogic.ProxyStatement<DTOBoxGuild>();
            return await db_guilds.FirstIfExistsAsync<DTOBoxGuild>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxGuild.FilterID, ObjectValue = id, Type = FilterType.Equal }
                }
            });

        }
    }
}
