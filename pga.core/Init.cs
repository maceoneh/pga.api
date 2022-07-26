using es.dmoreno.utils.dataaccess.db;
using pga.core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    static public class Init
    {
        public static string DataPath { get; set; } = "";

        static public async Task BuildEnvironment()
        {
            await BuidDataBase(); 
        }

        static private async Task BuidDataBase()
        {
            using (var db = new DataBaseLogic(new ConnectionParameters
            {
                File = DataPath + "pga.core.db",
                Type = DBMSType.SQLite
            }))
            {
                await db.Management.createAlterTableAsync<DTOUser>();
            }
        }
    }
}
