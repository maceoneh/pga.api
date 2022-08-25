using es.dmoreno.utils.dataaccess.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "appointments", FilePerTable = true)]
    public class DTOBoxAppointment
    {
        [Field(FieldName = "id", IsPrimaryKey = true, IsAutoincrement = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [Field(FieldName = "uuid", Type = ParamType.String)]
        public string UUID { get; set; }

        public List<DTOBoxEmployInAppointment> EmployeesInAppointment { get; set; } = null;

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

        [Field(FieldName = "agreed", Type = ParamType.Int32)]
        public EBoxAppointmentStatus Status { get; set; } = EBoxAppointmentStatus.InProgress;



        //static private readonly string[] FieldsNotClaim = new string[] {
        //                                "idcompany", "idsubcompany", "idrepairer", "idadministrator", "insureddni", "hour",
        //                                "descprovider", "insuredmail", "policynumber", "claiminternalcode", "claimnumber", "assistancenumber",
        //                                "agreedappointment", "pgamobilestatus", "descguild", "idmasterdetail", "insuredaddress", "insuredname", "insuredpopulation", "insuredprovince", "insuredpostalcode", "code", "appointmentdate", "description",
        //                                "insuredtel1", "insuredtel2", "insuredtel3", "insuredtelfax", "idclaim", "idfirm", "idguild", "descphase" };

        //static private readonly string[] FieldsClaim = new string[] {
        //                                "idcompany", "idsubcompany", "idrepairer", "idadministrator", "insureddni",
        //                                "descprovider", "insuredmail", "policynumber", "claiminternalcode", "claimnumber", "assistancenumber", "hour",
        //                                "agreedappointment", "pgamobilestatus", "descguild", "idmasterdetail", "insuredaddress", "insuredname", "insuredpopulation", "insuredprovince", "insuredpostalcode", "code", "appointmentdate", "description",
        //                                "insuredtel1", "insuredtel2", "insuredtel3", "insuredtelfax", "idclaim", "urgent_claim", "idfirm", "idguild", "descphase" };
    }
}
