using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.textplain;
using es.dmoreno.utils.security;
using es.dmoreno.utils.serialize;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class Box : IDisposable
    {
        public string UUID { get; private set; }

        internal string DataPath { get => Init.BoxesPath + @"\" + this.UUID; }

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

        public Box(string uuid)
        {
            this.Initialize(null, uuid);
        }

        internal Box(Box b)
        {
            this.Initialize(b, null);
        }

        private void Initialize(Box b, string uuid)
        {
            if (b != null)
            {
                this.UUID = b.UUID;
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
                if (string.IsNullOrWhiteSpace(this.UUID))
                {
                    throw new ArgumentException("UUID can't be empty");
                }
                this.BuildIfNecessary();
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
            if (!File.Exists(path + @"\version.json"))
            {
                using (var filehelper = new TextPlainFile(path + @"\version.json"))
                {
                    filehelper.set(JSon.serializeJSON<DTOBoxVersion>(new DTOBoxVersion { VersionToken = "0", LastUpdateDatabase = DateTime.Now }));
                }
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
                await this.DBLogic.Management.createAlterTableAsync<DTOBoxMasterData>();
                using (var filehelper = new TextPlainFile(path + @"\version.json"))
                {
                    filehelper.set(JSon.serializeJSON<DTOBoxVersion>(new DTOBoxVersion { VersionToken = Box.LastUpdateToken, LastUpdateDatabase = DateTime.Now }));
                }
            }
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
