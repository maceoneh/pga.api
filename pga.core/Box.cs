﻿using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.textplain;
using es.dmoreno.utils.security;
using es.dmoreno.utils.serialize;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class Box : IDisposable
    {
        /// <summary>
        /// Buzones con la estructura actualizada
        /// </summary>
        static private List<string> BoxesBuilded { get; } = new List<string>();

        /// <summary>
        /// Buzones con la base de datos actualizada
        /// </summary>
        static private List<string> BoxesUpdatedDB { get; } = new List<string>();

        public string UUID { get; private set; }

        public string AccessToken { get; private set; }

        internal DTOBoxSubject Subject { get; private set; } = null;

        /// <summary>
        /// Ruta al directorio raiz del box
        /// </summary>
        internal string DataPath { get => Init.BoxesPath + @"\" + this.UUID; }

        /// <summary>
        /// Ruta al directorio donde se guarara la información adicional de las actividades
        /// </summary>
        internal string ActivityPath { get => DataPath + @"\data\activity"; }

        internal ConnectionParameters ConnectionParameters
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DataPath))
                {
                    throw new Exception("DataPath is not set");
                }
                return new ConnectionParameters
                {
                    File = DataPath + @"\data.db",
                    Type = DBMSType.SQLite
                };
            }
        }

        public static string LastUpdateToken { get; private set; } = Token.generate(new ConfigToken { Length = 20, Letters = true, Numbers = true, UpperCase = true });

        private DataBaseLogic _db_logic = null;

        private bool _dispose_db_logic = true;

        private bool disposedValue;

        internal DataBaseLogic DBLogic { get => this._db_logic; }

        public Box(string uuid, string accessToken = "")
        {
            this.Initialize(null, uuid, accessToken);
        }

        internal Box(Box b)
        {
            this.Initialize(b, null, null);
        }

        private void Initialize(Box b, string uuid, string accessToken)
        {
            if (b != null)
            {
                this.UUID = b.UUID;
                this.AccessToken = b.AccessToken;
                this._db_logic = b._db_logic;
                this._dispose_db_logic = false;
                if (string.IsNullOrWhiteSpace(this.UUID))
                {
                    throw new ArgumentException("UUID can't be empty");
                }
            }
            else
            {
                this.UUID = uuid;
                this.AccessToken = accessToken;
                if (string.IsNullOrWhiteSpace(this.UUID))
                {
                    throw new ArgumentException("UUID can't be empty");
                }
                if (!BoxesBuilded.Contains(this.UUID))
                {
                    this.BuildIfNecessary();
                    BoxesBuilded.Add(this.UUID);
                }
                this._db_logic = new DataBaseLogic(new ConnectionParameters
                {
                    File = this.DataPath + @"\data.db",
                    Type = DBMSType.SQLite
                });
                this._dispose_db_logic = true;
            }
        }

        public void BuildIfNecessary()
        {
            var path = Init.BoxesPath + @"\" + this.UUID;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //Se crea el fichero de versiones
            if (!File.Exists(path + @"\version.json"))
            {
                using (var filehelper = new TextPlainFile(path + @"\version.json"))
                {
                    filehelper.set(JSon.serializeJSON<DTOBoxVersion>(new DTOBoxVersion { VersionToken = "0", LastUpdateDatabase = DateTime.Now }));
                }
            }
            //Se crean los subdirectorios de ficheros
            var data = path + @"\data\";
            if (!Directory.Exists(data))
            {
                Directory.CreateDirectory(data);
            }
            var data_activity = data + "activity";
            if (!Directory.Exists(data_activity))
            {
                Directory.CreateDirectory(data_activity);
            }
        }

        public async Task CreateUpdateDatabaseIfNecessaryAsync()
        {
            if (!BoxesUpdatedDB.Contains(this.UUID))
            {
                await this.CreateUpdateDatabase();
                BoxesUpdatedDB.Add(this.UUID);
            }
        }

        public async Task CreateUpdateDatabase()
        {
            var path = Init.BoxesPath + @"\" + this.UUID;
            string txt = null;
            using (var filehelper = new TextPlainFile(path + @"\version.json"))
            {
                txt = filehelper.get();
            }
            var update = string.IsNullOrWhiteSpace(txt);
            if (!update)
            {
                var data = JSon.deserializeJSON<DTOBoxVersion>(txt);
                update = !Box.LastUpdateToken.Equals(data.VersionToken);
            }
            if (update)
            {
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxAppointment>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxEmployInAppointment>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxMasterData>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxSubject>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxSubjectRoot>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxSubjectEmploy>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxSession>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxFile>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxCounter>();
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxMessage>();

                using (var filehelper = new TextPlainFile(path + @"\version.json"))
                {
                    filehelper.set(JSon.serializeJSON<DTOBoxVersion>(new DTOBoxVersion { VersionToken = Box.LastUpdateToken, LastUpdateDatabase = DateTime.Now }));
                }
            }
        }

        public async Task<DTOBoxSubject> WhoIs()
        {
            if (this.Subject == null)
            {
                this.Subject = await this.GetBoxSessionsHelper().WhoIs();
            }
            return this.Subject;
        }

        public BoxSubject GetBoxSubjectHelper()
        {
            return new BoxSubject(this);
        }

        public BoxSessions GetBoxSessionsHelper()
        {
            return new BoxSessions(this);
        }

        public BoxFile GetBoxFileHelper()
        {
            return new BoxFile(this);
        }

        public BoxCounters GetBoxCountersHelper()
        {
            return new BoxCounters(this);
        }

        public BoxMessage GetBoxMessageHelper()
        {
            return new BoxMessage(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                    if (this._dispose_db_logic)
                    {
                        if (this._db_logic != null)
                        {
                            this._db_logic.Dispose();
                            this._db_logic = null;
                        }
                        this._dispose_db_logic = false;
                    }
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~BoxBase()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
