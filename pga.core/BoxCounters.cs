using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxCounters
    {
        private Box Box { get; }

        internal BoxCounters(Box b)
        {
            this.Box = b;
        }

        internal async Task<int> GetNextAsync(EBoxCounterType t, string prefix = "")
        {
            if (prefix == null)
            {
                prefix = "";
            }
            var db_counters = await this.Box.DBLogic.ProxyStatement<DTOBoxCounter>();
            var c = await db_counters.FirstIfExistsAsync<DTOBoxCounter>(new StatementOptions { 
                Filters = new List<Filter> { 
                    new Filter { Name = DTOBoxCounter.FilterType, ObjectValue = (int)t, Type = FilterType.Equal },
                    new Filter { Name = DTOBoxCounter.FilterPrefix, ObjectValue = prefix, Type = FilterType.Equal }
                }
            });
            if (c == null)
            {
                c = new DTOBoxCounter {
                    Type = t,
                    Prefix = prefix,
                    Counter = 1
                };
                await db_counters.insertAsync(c);
            }
            else 
            {
                c.Counter++;
                await db_counters.updateAsync(c);
            }
            return c.Counter;
        }
    }
}
