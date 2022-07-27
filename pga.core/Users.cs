using pga.core.DTOs;
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
            return await (await this.DBLogic.ProxyStatement<DTOUser>()).insertAsync(u);
        }
    }
}
