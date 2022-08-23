using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxFile
    {
        private Box Box { get; }

        internal BoxFile(Box b)
        {
            this.Box = b;
        }

        /// <summary>
        /// Inserta un expediente en la base de datos
        /// </summary>
        /// <param name="f"></param>
        /// <remarks>Falta comprobar permisos</remarks>
        /// <returns></returns>
        public async Task<bool> CreateFile(DTOBoxFile f)
        {
            //Comprobar permisos
            //Se crean las referencias
            //Se crea al receptor del expediente
            var subjectshelper = this.Box.GetBoxSubjectHelper();
            if (!(await subjectshelper.ExistsAndLoadAsync(f.Receiver)))
            {
                f.Receiver = await subjectshelper.CreateSubjectAsync(f.Receiver);
            }
            f.RefReceiver = f.Receiver.ID;
            //Se crea el proveedor
            if (!(await subjectshelper.ExistsAndLoadAsync(f.Provider)))
            {
                f.Provider = await subjectshelper.CreateSubjectAsync(f.Provider);
            }
            f.RefProvider = f.Provider.ID;
            //Se crea el intermediario

            return false;
        }
    }
}
