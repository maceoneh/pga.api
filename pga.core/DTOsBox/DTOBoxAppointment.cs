using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "appointments", FilePerTable = true)]
    public class DTOBoxAppointment : IDataPermission
    {
        public const string TAG = "DTOBoxAppointment";
        public const string FilterExternalID = TAG + "ExternalID";
        public const string IdxExternalID = TAG + "ExternalID";

        [Field(FieldName = "id", IsPrimaryKey = true, IsAutoincrement = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [Field(FieldName = "uuid", Type = ParamType.String)]
        public string UUID { get; set; }

        [Filter(Name = FilterExternalID)]
        [Index(Name = IdxExternalID, Unique = true)]
        [Field(FieldName = "external_id", Type = ParamType.Int32)]
        internal int ExternalID { get; set; }

        public List<DTOBoxEmployInAppointment>? EmployeesInAppointment { get; set; } = null;

        [Field(FieldName = "date_from", Type = ParamType.DateTime)]
        public DateTime DateFrom { get; set; }

        [Field(FieldName = "date_to", Type = ParamType.DateTime)]
        public DateTime DateTo { get; set; }

        [Field(FieldName = "ref_receiver", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public int RefReceiver { get; set; } = int.MinValue;

        [Field(FieldName = "ref_file", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public int RefFile { get; set; } = int.MinValue;

        [Field(FieldName = "description", Type = ParamType.String)]
        public string? Description { get; set; }

        [Field(FieldName = "guild_description", Type = ParamType.String)]
        public string? GuildDescription { get; set; }

        [Field(FieldName = "agreed", Type = ParamType.Boolean, DefaultValue = false)]
        public bool Agreed { get; set; }

        [Field(FieldName = "status", Type = ParamType.Int32)]
        public EBoxAppointmentStatus Status { get; set; } = EBoxAppointmentStatus.InProgress;

        public string IDRecord => this.ID.ToString();

        internal DTOBoxAppointment CopyTo(DTOBoxAppointment a)
        {
            a.Agreed = this.Agreed;
            a.DateFrom = this.DateFrom;
            a.DateTo = this.DateTo;
            a.Description = this.Description;
            a.GuildDescription = this.GuildDescription;
            a.ID = this.ID;
            a.RefFile = this.RefFile;
            a.RefReceiver = this.RefReceiver;
            a.Status = this.Status;
            a.UUID = this.UUID;
            a.EmployeesInAppointment = new List<DTOBoxEmployInAppointment>(this.EmployeesInAppointment.Count);
            foreach (var item in this.EmployeesInAppointment)
            {
                a.EmployeesInAppointment.Add(item.CopyTo(new DTOBoxEmployInAppointment()));
            }
            return a;
        }
    }
}
