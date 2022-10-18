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
        public const string FilterID = TAG + "ID";
        public const string IdxExternalID = TAG + "ExternalID";

        [Filter(Name = FilterID)]
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
            a.UUID = this.UUID;
            a.EmployeesInAppointment = new List<DTOBoxEmployInAppointment>(this.EmployeesInAppointment.Count);
            foreach (var item in this.EmployeesInAppointment)
            {
                a.EmployeesInAppointment.Add(item.CopyTo(new DTOBoxEmployInAppointment()));
            }
            return a;
        }
    }

    [Table(Name = "appointments_archive", FilePerTable = true)]
    public class DTOBoxAppointmentArchive : DTOBoxAppointment
    {
        [Filter(Name = FilterID)]
        [Field(FieldName = "id", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        internal new int ID { get; set; }

        public new List<DTOBoxEmployInAppointmentArchive>? EmployeesInAppointment { get; set; } = null;
    }

    public class DTOBoxAppointmentExtend : DTOBoxAppointment
    {
        public string? DescPhase { get; set; }
        public string? DescProvider { get; set; }
        public string? InsuredAddress { get; set; }
        public string? InsuredDNI { get; set; }
        public string? InsuredMail { get; set; }
        public string? InsuredName { get; set; }
        public string? InsuredPopulation { get; set; }
        public string? InsuredPostalCode { get; set; }
        public string? InsuredProvince { get; set; }
        public string? InsuredTel1 { get; set; }
        public string? InsuredTel2 { get; set; }
        public string? InsuredTel3 { get; set; }
        public string? InsuredTelFax { get; set; }
        public string? PolicyNumber { get; set; }
        public int IDAdministrator { get; set; }
        public int IDFirm { get; set; }
        public int IDClaim { get; set; }
        public int IdCompany { get; set; }
        public int IdSubCompany { get; set; }
        public int IDGuild { get; set; }
        public int IDMasterDetail { get; set; }
        public int IdRepairer { get; set; }
        public bool UrgentClaim { get; set; }

        public DTOBoxAppointmentExtend(DTOBoxAppointment a)
        {
            a.CopyTo(this);
        }
    }
}
