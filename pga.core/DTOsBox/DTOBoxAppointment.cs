﻿using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.permissions;
using Newtonsoft.Json;
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
        [Field(FieldName = "external_id", Type = ParamType.String)]
        public string? ExternalID { get; set; }

        [Field(FieldName = "external_id_master_detail", Type = ParamType.String)]
        public string? ExternalIDMasterDetail { get; set; }

        [Field(FieldName = "code", Type = ParamType.String)]
        public string? Code { get; set; }

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

        [Field(FieldName = "ref_guild", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefGuild { get; set; }

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
            a.ExternalID = this.ExternalID;
            a.ExternalIDMasterDetail = this.ExternalIDMasterDetail;
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
        public string? ContactPhone1 { get; set; }
        public string? ContactPhone2 { get; set; }
        public string? ContactPhone3 { get; set; }
        public string? PolicyNumber { get; set; }
        public int IDAdministrator { get; set; } = int.MinValue;
        public int IDFirm { get; set; } = int.MinValue;
        public int IDClaim { get; set; } = int.MinValue;
        public int IdCompany { get; set; } = int.MinValue;
        public int IdSubCompany { get; set; } = int.MinValue;
        public int IdRepairer { get; set; } = int.MinValue;
        public int IDGuild { get; set; } = int.MinValue;
        public bool UrgentFile { get; set; }
        public string? FileIntermediaryNumber { get; set; }
        public string? FileNumber { get; set; }
        public string? FileCode { get; set; }
        
        public DTOBoxAppointmentExtend(DTOBoxAppointment a)
        {
            a.CopyTo(this);
        }
    }
}
