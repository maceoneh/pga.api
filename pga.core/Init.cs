using es.dmoreno.utils.apps.masterrecords;
using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.security;
using pga.core.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    static public class Init
    {
        public static string DataPath { get; set; } = "";
        public static string BoxesPath { get => DataPath + "boxes"; }

        internal static ConnectionParameters ConnectionParameters
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Init.DataPath))
                {
                    throw new Exception("DataPath is not set");
                }
                return new ConnectionParameters
                {
                    File = DataPath + "pga.core.db",
                    Type = DBMSType.SQLite
                };
            }
        }

        static public async Task BuildEnvironment()
        {
            await BuildCoreDataBase();
            await InitializeData();
            await BuildBoxes();
        }

        static private async Task InitializeData()
        {
            var mrhelper = new MasterData(Init.ConnectionParameters);
            var user = await mrhelper.GetUserAdminMD5();
            var password = await mrhelper.GetPasswordAdminMD5();
            if (string.IsNullOrWhiteSpace(user) && string.IsNullOrWhiteSpace(password))
            {
                await mrhelper.SetUserAdminMD5(MD5Utils.GetHash("default_admin"));
                await mrhelper.SetPasswordAdminMD5(MD5Utils.GetHash("default_password"));
            }
        }

        static private async Task BuildCoreDataBase()
        {
            //Se crea la base de datos del core
            using (var db = new DataBaseLogic(new ConnectionParameters
            {
                File = DataPath + "pga.core.db",
                Type = DBMSType.SQLite
            }))
            {
                await db.Management.createAlterTableAsync<DTOUser>();
                await db.Management.createAlterTableAsync<DTOUserProfile>();
                await db.Management.createAlterTableAsync<DTOMasterData>();
            }
        }

        static private async Task BuildBoxes()
        {
            if (!Directory.Exists(DataPath + "boxes"))
            {
                Directory.CreateDirectory(DataPath + "boxes");
            }
        }
    }
}
