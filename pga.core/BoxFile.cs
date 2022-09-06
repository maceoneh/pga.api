using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOsBox;
using SQLitePCL;
using System;
using System.Collections.Generic;
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
        /// Comprueba si el expediente indicado existe
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private async Task<bool> IsOpenAndLoad(DTOBoxFile f)
        {
            var db_files = await this.Box.DBLogic.ProxyStatement<DTOBoxFile>();
            //Se comprueba si existe un expediente abierto con el UUID indicado
            if (!string.IsNullOrWhiteSpace(f.UUID))
            {
                var file_by_uuid = await db_files.FirstIfExistsAsync<DTOBoxFile>(new StatementOptions
                {
                    Filters = new List<Filter> {
                        new Filter { Name = DTOBoxFile.FilterUUID, ObjectValue = f.UUID, Type = FilterType.Equal }
                    }
                });
                if (file_by_uuid != null)
                {
                    if (file_by_uuid.Status == EBoxFileStatus.InProgress)
                    {
                        file_by_uuid.CopyTo(f);
                        return true;
                    }
                }
            }
            else
            {
                //Se busca el receptor
                var subjectshelper = this.Box.GetBoxSubjectHelper();
                if (!(await subjectshelper.ExistsAndLoadAsync(f.Receiver)))
                {
                    //Si el receptor no existe el expediente tampoco aunque exista uno con mismo numero
                    return false;
                }
                //Se comprueba si existe un expediente con el mismo numero, receptor, proveedor e interviniente y esta abierto
                var filters = new List<Filter> { 
                    new Filter { Name = DTOBoxFile.FilterNumber, ObjectValue = f.Number, Type = FilterType.Equal },
                    new Filter { Name = DTOBoxFile.FilterRefReceiver, ObjectValue = f.Receiver.ID, Type = FilterType.Equal},
                    new Filter { Name = DTOBoxFile.FilterStatus, ObjectValue = EBoxFileStatus.InProgress, Type = FilterType.Equal}
                };
                if (f.Provider != null)
                {
                    if (!(await subjectshelper.ExistsAndLoadAsync(f.Provider)))
                    {
                        //Si el proveedor no existe el expediente tampoco aunque exista uno con mismo numero
                        return false;
                    }
                    filters.Add(
                        new Filter { Name = DTOBoxFile.FilterRefProvider, ObjectValue = f.Provider.ID, Type = FilterType.Equal }
                    );
                }
                var file_by_receiver = await db_files.FirstIfExistsAsync<DTOBoxFile>(new StatementOptions { Filters = filters });
                if (file_by_receiver != null)
                {
                    file_by_receiver.CopyTo(f);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inserta un expediente en la base de datos
        /// </summary>
        /// <param name="f"></param>
        /// <remarks>Falta comprobar permisos</remarks>
        /// <returns></returns>
        public async Task<DTOBoxFile?> CreateFile(DTOBoxFile f, bool check_if_open = true)
        {
            //Comprobar permisos
            this.ValidateCorrectFieldsForCreate(f);
            //Se comprueba si el expediente ya esta abierto
            if (check_if_open)
            {
                if (await this.IsOpenAndLoad(f))
                {
                    return f;
                }
            }
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
            if (f.Date.Ticks == 0)
            {
                f.Date = DateTime.Now;
            }
            f.Status = EBoxFileStatus.InProgress;
            f.UUID = Guid.NewGuid().ToString();
            var counterhelper = this.Box.GetBoxCountersHelper();
            f.InternalNumber = (await counterhelper.GetNextAsync(EBoxCounterType.AppointmentInternalNumber)).ToString();
            //Se inserta el expediente en la base de datos
            var db_file = await this.Box.DBLogic.ProxyStatement<DTOBoxFile>();
            if (await db_file.insertAsync(f))
            {
                f.ID = db_file.lastID;
                await this.Box.GetBoxActivityHelper().AddAsync(new DTOBoxActivity
                {
                    Flow = EBoxActivityFlow.In,
                    RefAppointment = f.ID,
                    Type = EBoxActivityType.CreateAppointment,
                    Activity = ""
                });
                return f;
            }
            else
            {
                return null;
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

        /// <summary>
        /// Agrega una cita en el expediente indicado
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public async Task<DTOBoxAppointment?> AddAppointmentAsync(DTOBoxAppointment a, DTOBoxFile f)
        {
            //Se completa información
            a.UUID = Guid.NewGuid().ToString();
            a.RefFile = f.ID;
            a.RefReceiver = f.RefReceiver;
            //Se agregan registros en la base de datos
            var db_appointments = await this.Box.DBLogic.ProxyStatement<DTOBoxAppointment>();
            if (await db_appointments.insertAsync(a))
            {
                a.ID = db_appointments.lastID;                
                if (a.EmployeesInAppointment != null)
                {
                    var db_employeesinappointmet = await this.Box.DBLogic.ProxyStatement<DTOBoxEmployInAppointment>();
                    foreach (var item in a.EmployeesInAppointment)
                    {
                        item.RefAppointment = a.ID;
                        if (item.Employ != null)
                        {
                            item.RefEmploy = item.Employ.ID;
                        }
                        else
                        {
                            item.RefEmploy = int.MinValue;
                        }
                        if (!await db_employeesinappointmet.insertAsync(item))
                        {
                            return null;
                        }
                    }
                }
                return a;
            }
            return null;
        }
    }
}
