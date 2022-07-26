using es.dmoreno.utils.apps.masterrecords;
using es.dmoreno.utils.dataaccess.db;
using pga.core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    internal class MasterData : MasterRecordsTable<DTOMasterData>
    {
        private const string PARAMUserAdminMD5 = "UserAdminMD5";
        private const string PARAMPasswordAdminMD5 = "PasswordAdminMD5";

        public async Task<string> GetUserAdminMD5()
        {
            return (await this.getAsync(PARAMUserAdminMD5, true)).Value;
        }

        internal async Task SetUserAdminMD5(string user)
        {
            var p = await this.getAsync(PARAMUserAdminMD5, true);
            p.Value = user;
            await this.setAsync(p);
        }

        public async Task<string> GetPasswordAdminMD5()
        {
            return (await this.getAsync(PARAMPasswordAdminMD5, true)).Value;
        }

        internal async Task SetPasswordAdminMD5(string user)
        {
            var p = await this.getAsync(PARAMPasswordAdminMD5, true);
            p.Value = user;
            await this.setAsync(p);
        }

        internal MasterData(ConnectionParameters param) : base(param)
        {
        }
    }
}
