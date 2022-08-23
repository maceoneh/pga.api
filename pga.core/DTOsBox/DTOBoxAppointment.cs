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

        [Field(FieldName = "date_from", Type = ParamType.DateTime)]
        public DateTime DateFrom { get; set; }

        [Field(FieldName = "date_to", Type = ParamType.DateTime)]
        public DateTime DateTo { get; set; }

        [Field(FieldName = "ref_receiver", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public int RefReceiver { get; set; }

        [Field(FieldName = "ref_file", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public int RefFile { get; set; }

        [Field(FieldName = "description", Type = ParamType.String)]
        public string Description { get; set; }

        [Field(FieldName = "description", Type = ParamType.String)]
        public string GuildDescription { get; set; }

        [Field(FieldName = "description", Type = ParamType.Boolean, DefaultValue = false)]
        public bool Agreed { get; set; }

        public EBoxAppointmentStatus Status { get; set; }



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
