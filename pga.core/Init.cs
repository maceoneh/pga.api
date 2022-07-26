﻿using es.dmoreno.utils.apps.masterrecords;
using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.security;
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
            await BuildDataBase();
            await InitializeData();
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

        static private async Task BuildDataBase()
        {
            using (var db = new DataBaseLogic(new ConnectionParameters
            {
                File = DataPath + "pga.core.db",
                Type = DBMSType.SQLite
            }))
            {
                await db.Management.createAlterTableAsync<DTOUser>();
                await db.Management.createAlterTableAsync<DTOMasterData>();
            }
        }


    }
}
