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
            this.ValidateCorrectFieldsForCreate(f);
            //Se crean las referencias
            //Se crea al receptor del expediente
            var subjectshelper = this.Box.GetBoxSubjectHelper();
            if (!(await subjectshelper.ExistsAndLoadAsync(f.Receiver)))
            {
                f.Receiver = await subjectshelper.CreateSubjectAsync(f.Receiver);
            }
            f.RefReceiver = f.Receiver.ID;
            //Se crea el proveedor si fuera necesario
            if (f.Provider != null)
            {                
                if (!(await subjectshelper.ExistsAndLoadAsync(f.Provider)))
                {
                    f.Provider = await subjectshelper.CreateSubjectAsync(f.Provider);
                }
                f.RefProvider = f.Provider.ID;
            }
            else
            {
                f.RefProvider = int.MinValue;
            }
            //Se crea el intermediario si fuera necesario
            if (f.Intermediary != null)
            {                
                if (!(await subjectshelper.ExistsAndLoadAsync(f.Intermediary)))
                {
                    f.Intermediary = await subjectshelper.CreateSubjectAsync(f.Intermediary);
                }
                f.RefIntermediary = f.Intermediary.ID;
            }
            else
            {
                f.RefIntermediary = int.MinValue;
            }
            //Se rellenan algunos datos
            f.UUID = Guid.NewGuid().ToString();
            //Se inserta el expediente en la base de datos
            var db_file = await this.Box.DBLogic.ProxyStatement<DTOBoxFile>();
            if (await db_file.insertAsync(f))
            {
                f.ID = db_file.lastID;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Chequea si tiene todos los campos necesarios para crear un expediente
        /// </summary>
        /// <param name="f"></param>
        internal void ValidateCorrectFieldsForCreate(DTOBoxFile f)
        {
            if (f == null)
            {
                throw new ArgumentException("DTOBoxFile can't be NULL");
            }

            if (f.Receiver == null)
            {
                throw new ArgumentException("Receiver can't be NULL");
            }           
        }
    }
}
