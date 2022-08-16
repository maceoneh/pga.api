using es.dmoreno.utils.apps.masterrecords;
using es.dmoreno.utils.dataaccess.db;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    internal class BoxMasterData : MasterRecordsTable<DTOBoxMasterData>
    {
        private const string PARAMUserAdmin = "UserAdmin";
        private const string PARAMPasswordAdmin = "PasswordAdmin";

        public async Task<string> GetUserAdminMD5()
        {
            return (await this.getAsync(PARAMUserAdmin, true)).Value;
        }

        internal async Task SetUserAdminMD5(string user)
        {
            var p = await this.getAsync(PARAMUserAdmin, true);
            p.Value = user;
            await this.setAsync(p);
        }

        public async Task<string> GetPasswordAdminMD5()
        {
            return (await this.getAsync(PARAMPasswordAdmin, true)).Value;
        }

        internal async Task SetPasswordAdminMD5(string user)
        {
            var p = await this.getAsync(PARAMPasswordAdmin, true);
            p.Value = user;
            await this.setAsync(p);
        }

        private Box Box { get; set; }

        internal BoxMasterData(Box b) : base(b.ConnectionParameters)
        {
            this.Box = b;
        }
    }
}
