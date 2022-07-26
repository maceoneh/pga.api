using es.dmoreno.utils.dataaccess.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core
{
    abstract internal class Base : IDisposable
    {
        private DataBaseLogic _db_logic = null;
        private bool _dispose_db_logic = false;
        private bool disposedValue;

        protected DataBaseLogic DBLogic { get => this._db_logic; }

        public Base(Base b)
        {
            if (b != null)
            {
                this._db_logic = b._db_logic;
                this._dispose_db_logic = false;
            }
            else
            {               
                this._db_logic = new DataBaseLogic(Init.ConnectionParameters);
                this._dispose_db_logic = true;
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
                        this._db_logic.Dispose();
                        this._db_logic = null;
                    }
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~Base()
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
