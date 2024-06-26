﻿using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.permissions;
using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using pga.core.Exceptions;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            f.Code = (await counterhelper.GetNextAsync(EBoxCounterType.AppointmentInternalNumber)).ToString();
            //Se inserta el expediente en la base de datos
            var db_file = await this.Box.DBLogic.ProxyStatement<DTOBoxFile>();
            if (await db_file.insertAsync(f))
            {
                f.ID = db_file.lastID;
                await this.Box.GetBoxMessageHelper().AddAsync(new DTOBoxMessage
                {
                    Flow = EBoxMessageFlow.In,
                    RefFile = f.ID,
                    Type = EBoxMessageType.CreateAppointment,
                    Message = ""
                });
                //Se agregan los permisos al registro
                var users_group = await this.Box.GetUsersGroupAsync();
                DTOUUIDRecordPermision? record_permissions = null;
                if (users_group != null)
                {
                    record_permissions = new DTOUUIDRecordPermision
                    {
                        UUID = users_group.UUID,
                        CanRead = true,
                        CanWrite = false
                    };
                }
                using (var permissionshelper = new Permissions(this.Box.DataPath))
                {
                    await permissionshelper.AddDataPermissionAsync<DTOBoxFile>(f,
                        new DTORecordPermission
                        {
                            UUIDOwner = (await this.Box.WhoIs()).UUID,
                            UUIDRecordPermissions = new DTOUUIDRecordPermision[] {
                                record_permissions
                            }
                        });
                }
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
            //a.ExternalID = Token.getRandomID();
            a.RefFile = f.ID;
            a.RefReceiver = f.RefReceiver;
            //Se obtiene el codigo
            var countershelper = this.Box.GetBoxCountersHelper();
            var codenumber = await countershelper.GetNextAsync(EBoxCounterType.AppointmentInternalNumber, a.DateTo.Year.ToString());
            a.Code = a.DateTo.Year.ToString() + "/" + codenumber.ToString();
            //Se obtienen los datos del gremio
            if (a.GuildDescription != null)
            {
                var guildhelper = this.Box.GetBoxGuildHelper();
                var guilds = await guildhelper.GetListAsync();
                //Se busca el id del gremio y si no se crea
                var guild = guilds.Where(reg => reg.Name == a.GuildDescription.ToUpper().Trim()).FirstOrDefault();
                if (guild == null)
                {
                    guild = await guildhelper.AddSync(a.GuildDescription);
                }
                a.RefGuild = guild.ID;
            }
            else
            {
                a.RefGuild = int.MinValue;
            }
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
                        else
                        {
                            //Se guarda la actividad
                            await this.Box.GetBoxMessageHelper().AddAsync(new DTOBoxMessage
                            {
                                RefFile = f.ID,
                                RefAppointment = a.ID,
                                Flow = EBoxMessageFlow.In,
                                Type = EBoxMessageType.CreateAppointment
                            });
                        }
                    }
                }
                return a;
            }
            return null;
        }

        /// <summary>
        /// Obtiene una cita desde un Id externo
        /// </summary>
        /// <param name="external_id"></param>
        /// <returns></returns>
        public async Task<DTOBoxAppointment?> GetAppointmentByExternalIDAsync(int external_id)
        {
            var db_appointment = await this.Box.DBLogic.ProxyStatement<DTOBoxAppointment>();
            var appointment = await db_appointment.FirstIfExistsAsync<DTOBoxAppointment>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxAppointment.FilterExternalID, ObjectValue = external_id, Type = FilterType.Equal }
                }
            });
            if (appointment != null)
            {
                var db_appointment_employess = await this.Box.DBLogic.ProxyStatement<DTOBoxEmployInAppointment>();
                var employees_in_appointment = await db_appointment_employess.selectAsync<DTOBoxEmployInAppointment>(new StatementOptions
                {
                    Filters = new List<Filter>
                    {
                        new Filter { Name = DTOBoxEmployInAppointment.FilterRefAppointment, ObjectValue = appointment.ID, Type = FilterType.Equal }
                    }
                });
                if (employees_in_appointment != null)
                {
                    var subjecthelper = this.Box.GetBoxSubjectHelper();
                    foreach (var item in employees_in_appointment)
                    {
                        item.Employ = await subjecthelper.GetByIDAsync(item.RefEmploy);
                    }
                    appointment.EmployeesInAppointment = employees_in_appointment;
                }
            }
            //Se comprueba si el usuario tiene permisos para acceder al registro
            using (var permissionshelper = new Permissions(this.Box.DataPath))
            {
                await permissionshelper.CheckCanReadPermissionAsync(appointment, (await this.Box.WhoIs()).UUID, true);
            }
            return appointment;
        }

        /// <summary>
        /// Asocia una actividad de cita descargada a la cita pasada por parámetro
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public async Task<bool> AddStatusDownloadedAsync(DTOBoxAppointment a)
        {
            var action_subject = await this.Box.WhoIs();
            return await this.Box.GetBoxMessageHelper().AddAsync(new DTOBoxMessage
            {
                Flow = EBoxMessageFlow.In,
                RefAppointment = a.ID,
                RefFile = a.RefFile,
                RefSubject = action_subject.ID,
                Type = EBoxMessageType.DownloadedAppointment
            });
        }

        /// <summary>
        /// Agrega un mensaje genérico a un appointment
        /// </summary>
        /// <param name="a"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> AddMessageToAppointmentAsync(DTOBoxAppointment a, DTOBoxMessage msg)
        {
            await this.Box.CheckPermissionAndFireAsync(BoxDefinitions.ActionCreate, BoxDefinitions.EntityMessage);
            var action_subject = await this.Box.WhoIs();
            msg.RefAppointment = a.ID;
            msg.RefFile = a.RefFile;
            msg.RefSubject = action_subject.ID;
            if (msg.Date == null)
            {
                msg.Date = DateTime.Now;
            }
            msg.Flow = EBoxMessageFlow.In;
            return await this.Box.GetBoxMessageHelper().AddAsync(msg, fill_date: false);
        }

        private async Task<List<DTOBoxAppointment>> CompleteAppointments(List<DTOBoxAppointment> l)
        {
            //Se obtiene el listado de expedientes relacionados con los appointments
            var ids = new List<int>(l.Count);
            foreach (var item in l)
            {
                ids.Add(item.RefFile);
            }
            //Se carga el listado de expedientes al que se hace referencia
            var db_files = await this.Box.DBLogic.ProxyStatement<DTOBoxFile>();
            var files = await db_files.selectAsync<DTOBoxFile>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxFile.FilterID, ObjectValue = ids, Type = FilterType.In }
                }
            });
            //Se cargan los subjects necesarios
            var ref_subjects = new List<int>();
            foreach (var item in files)
            {
                if (item.RefReceiver > int.MinValue)
                {
                    if (!ref_subjects.Contains(item.RefReceiver))
                    {
                        ref_subjects.Add(item.RefReceiver);
                    }
                }
                if (item.RefProvider > int.MinValue)
                {
                    if (!ref_subjects.Contains(item.RefProvider))
                    {
                        ref_subjects.Add(item.RefProvider);
                    }
                }
                if (item.RefIntermediary > int.MinValue)
                {
                    if (!ref_subjects.Contains(item.RefIntermediary))
                    {
                        ref_subjects.Add(item.RefIntermediary);
                    }
                }
            }
            var db_subjects = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
            var subjects = await db_subjects.selectAsync<DTOBoxSubject>(new StatementOptions
            {
                Filters = new List<Filter> {
                    new Filter { Name = DTOBoxSubject.FilterID, ObjectValue = ref_subjects, Type = FilterType.In }
                }
            });
            //Se carga la información extendida
            var result = new List<DTOBoxAppointment>(l.Count);
            foreach (var item in l)
            {
                var file = files.Where(reg => reg.ID == item.RefFile).FirstOrDefault();
                var new_appointment = new DTOBoxAppointmentExtend(item);
                if (file != null)
                {
                    new_appointment.DescPhase = "";
                    file.Receiver = subjects.Where(reg => reg.ID == file.RefReceiver).FirstOrDefault();
                    if (file.Receiver != null)
                    {
                        new_appointment.InsuredDNI = "";
                        new_appointment.InsuredMail = file.Receiver.eMail;
                        new_appointment.InsuredName = file.Receiver.Name;
                    }
                    else
                    {
                        new_appointment.InsuredDNI = "";
                        new_appointment.InsuredMail = "";
                        new_appointment.InsuredName = "";
                    }
                    file.Provider = subjects.Where(reg => reg.ID == file.RefProvider).FirstOrDefault();
                    if (file.Provider != null)
                    {
                        new_appointment.DescProvider = file.Provider.Name;
                        new_appointment.IdCompany = file.Provider.ID;
                        new_appointment.IdSubCompany = -1;

                    }
                    file.Intermediary = subjects.Where(reg => reg.ID == file.RefIntermediary).FirstOrDefault();
                    if (file.Intermediary != null)
                    {
                        new_appointment.DescProvider = file.Intermediary.Name;
                        new_appointment.IdCompany = file.Intermediary.ID;
                        if (file.Provider != null)
                        {
                            new_appointment.IdSubCompany = file.Provider.ID;
                        }
                    }
                    try { new_appointment.IDClaim = Convert.ToInt32(file.ExternalID); } catch { new_appointment.IDClaim = int.MinValue; }
                    try { new_appointment.IDFirm = Convert.ToInt32(file.ExternalIDFirm); } catch { new_appointment.IDFirm = int.MinValue; }
                    new_appointment.InsuredAddress = file.Address;
                    new_appointment.InsuredPopulation = file.Population;
                    new_appointment.InsuredProvince = file.Province;
                    new_appointment.InsuredPostalCode = file.PostalCode;
                    new_appointment.ContactPhone1 = file.PhoneContact1;
                    new_appointment.ContactPhone2 = file.PhoneContact2;
                    new_appointment.ContactPhone3 = file.PhoneContact3;
                    new_appointment.PolicyNumber = file.Policy;
                    new_appointment.UrgentFile = file.Urgent;
                    new_appointment.IDGuild = item.RefGuild;
                }
                result.Add(new_appointment);
            }
            return result;
        }

        public async Task<List<DTOBoxAppointment>> GetAppointmentsByEmployAsync(string uuid, bool use_archive, bool extend)
        {
            var subjecthelper = this.Box.GetBoxSubjectHelper();
            //Se obtiene el sujeto
            var subject = await subjecthelper.GetByUUIDAsync(uuid);
            //Se comprueba si es un empleado
            if (await subjecthelper.IsEmployAsync(subject))
            {
                //Se obtienen los appointments con los que esta asociado
                var db_employ_in_appointment = await this.Box.DBLogic.ProxyStatement<DTOBoxEmployInAppointment>();
                var employ_in_appointment = await db_employ_in_appointment.selectAsync<DTOBoxEmployInAppointment>(new StatementOptions
                {
                    Filters = new List<Filter> {
                        new Filter { Name = DTOBoxEmployInAppointment.FilterRefEmploy, ObjectValue = subject.ID, Type = FilterType.Equal }
                    }
                });
                var ids = new List<int>();
                foreach (var item in employ_in_appointment)
                {
                    ids.Add(item.RefAppointment);
                }
                var db_appointmets = await this.Box.DBLogic.ProxyStatement<DTOBoxAppointment>();
                var appointments = await db_appointmets.selectAsync<DTOBoxAppointment>(new StatementOptions
                {
                    Filters = new List<Filter> {
                        new Filter { Name = DTOBoxAppointment.FilterID, ObjectValue = ids, Type = FilterType.In }
                    }
                });
                //Se asocian los appointments a los empleados
                foreach (var item in employ_in_appointment)
                {
                    var appointment = appointments.Where(reg => reg.ID == item.RefAppointment).FirstOrDefault();
                    if (appointment != null)
                    {
                        if (appointment.EmployeesInAppointment == null)
                        {
                            appointment.EmployeesInAppointment = new List<DTOBoxEmployInAppointment>();
                        }
                        appointment.EmployeesInAppointment.Add(item);
                    }
                }
                //Se eliminan los resultados que no pertenecen al usuario que hace la consulta
                using (var permissionhelper = new Permissions(this.Box.DataPath))
                {
                    appointments = await permissionhelper.FilterByDataPermission<DTOBoxAppointment>(appointments, (await this.Box.WhoIs()).UUID);
                }
                if (use_archive)
                {
                    var db_employ_in_appointment_archive = await this.Box.DBLogic.ProxyStatement<DTOBoxEmployInAppointmentArchive>();
                    var employ_in_appointment_archive = await db_employ_in_appointment_archive.selectAsync<DTOBoxEmployInAppointmentArchive>(new StatementOptions
                    {
                        Filters = new List<Filter> {
                        new Filter { Name = DTOBoxEmployInAppointment.FilterRefEmploy, ObjectValue = subject.ID, Type = FilterType.Equal }
                    }
                    });
                    ids.Clear();
                    foreach (var item in employ_in_appointment_archive)
                    {
                        ids.Add(item.RefAppointment);
                    }
                    var db_appointmets_archive = await this.Box.DBLogic.ProxyStatement<DTOBoxAppointmentArchive>();
                    var appointments_archive = await db_appointmets_archive.selectAsync<DTOBoxAppointmentArchive>(new StatementOptions
                    {
                        Filters = new List<Filter> {
                            new Filter { Name = DTOBoxAppointmentArchive.FilterID, ObjectValue = ids, Type = FilterType.In }
                        }
                    });
                    //Se asocian los appointments a los empleados
                    foreach (var item in employ_in_appointment_archive)
                    {
                        var appointment = appointments_archive.Where(reg => reg.ID == item.RefAppointment).FirstOrDefault();
                        if (appointment != null)
                        {
                            if (appointment.EmployeesInAppointment == null)
                            {
                                appointment.EmployeesInAppointment = new List<DTOBoxEmployInAppointmentArchive>();
                            }
                            appointment.EmployeesInAppointment.Add(item);
                        }
                    }
                    //Se eliminan los resultados que no pertenecen al usuario que hace la consulta
                    using (var permissionhelper = new Permissions(this.Box.DataPath))
                    {
                        appointments_archive = await permissionhelper.FilterByDataPermission<DTOBoxAppointmentArchive>(appointments_archive, (await this.Box.WhoIs()).UUID);
                    }
                    //Se agregan al listado principal
                    appointments.AddRange(appointments_archive);
                }
                //Se carga el subject correspodiente
                var employees = await subjecthelper.GetEmployeesAsync();
                foreach (var appointment in appointments)
                {
                    if (appointment.EmployeesInAppointment != null)
                    {
                        foreach (var item in appointment.EmployeesInAppointment)
                        {
                            item.Employ = employees.Where(reg => reg.ID == item.RefEmploy).FirstOrDefault().CopyTo(new DTOBoxSubject());
                        }
                    }
                }
                //Se extiende la información
                if (extend)
                {
                    var appointments_extend = await this.CompleteAppointments(appointments);
                    appointments = appointments_extend;
                }

                return appointments;
            }
            else
            {
                throw new RegisterNotExistsException("Not exists employ with UUID " + uuid);
            }
        }
    }
}
