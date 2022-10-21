using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using es.dmoreno.utils.permissions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace pga.core.DTOsBox
{
    [Table(Name = "files", FilePerTable = true)]
    public class DTOBoxFile : IDataPermission
    {
        public const string TAG = "DTOBoxFile";
        public const string FilterID = TAG + "ID";
        public const string FilterUUID = TAG + "UUID";
        public const string FilterRefReceiver = TAG + "RefReceiver";
        public const string FilterRefProvider = TAG + "RefProvider";
        public const string FilterRefIntermediary = TAG + "RefIntermediary";
        public const string FilterPolicy = TAG + "Policy";
        public const string FilterDate = TAG + "Date";
        public const string FilterAddress = TAG + "Address";
        public const string FilterPostalCode = TAG + "PostalCode";
        public const string FilterProvince = TAG + "Province";
        public const string FilterPopulation = TAG + "Population";
        public const string FilterStatus = TAG + "Status";
        public const string FilterNumber = TAG + "Number";
        public const string FilterInternalNumber = TAG + "NumberInternal";
        public const string FilterExternalID = TAG + "ExternalID";
        public const string IdxUUID = TAG + "UUID";
        public const string IdxRefReceiver = TAG + "RefInsured";
        public const string IdxRefProvider = TAG + "RefProvider";
        public const string IdxRefIntermediary = TAG + "RefIntermediary";
        public const string IdxPolicy = TAG + "Policy";
        public const string IdxDate = TAG + "Date";        
        public const string IdxAllAddressFields = TAG + "AllAddressFields";
        public const string IdxAddress = TAG + "Address";
        public const string IdxEmail = TAG + "EMail";
        public const string IdxNumber = TAG + "Number";
        public const string IdxInternalNumber = TAG + "InternalNumber";
        public const string IdxExternalID = TAG + "ExternalID";

        /// <summary>
        /// Listado de citas asociadas a un expediente
        /// </summary>
        [JsonPropertyName("appointments")]
        public List<DTOBoxAppointment>? AppointmentList { get; internal set; } = null;


        [Filter(Name = FilterRefReceiver)]
        [Index(Name = IdxRefReceiver)]
        [Field(FieldName = "ref_receiver", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefReceiver { get; set; }

        [JsonPropertyName("receiver")]
        public DTOBoxSubject? Receiver { get; set; } = null;

        [Filter(Name = FilterRefProvider)]
        [Index(Name = IdxRefProvider)]
        [Field(FieldName = "ref_provider", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefProvider { get; set; }

        [JsonPropertyName("provider")]
        
        public DTOBoxSubject? Provider { get; set; } = null;

        [Filter(Name = FilterRefIntermediary)]
        [Index(Name = IdxRefIntermediary)]
        [Field(FieldName = "ref_intermediary", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefIntermediary { get; set; }
        
        public DTOBoxSubject? Intermediary { get; set; } = null;

        [Filter(Name = FilterID)]
        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [JsonPropertyName("uuid")]
        [Index(Name = IdxUUID, Unique = true)]
        [Filter(Name = FilterUUID)]
        [Field(FieldName = "uuid", Type = ParamType.String)]
        public string? UUID { get; set; }

        [JsonPropertyName("number")]
        [Index(Name = IdxNumber)]
        [Filter(Name = FilterNumber)]
        [Field(FieldName = "number", Type = ParamType.String)]
        public string Number { get; set; }

        [JsonPropertyName("code")]
        [Index(Name = IdxInternalNumber)]
        [Filter(Name = FilterInternalNumber)]
        [Field(FieldName = "code", Type = ParamType.String)]
        public string? Code { get; set; }

        [JsonPropertyName("externalID")]
        [Index(Name = IdxExternalID)]
        [Filter(Name = FilterExternalID)]
        [Field(FieldName = "external_id", Type = ParamType.String)]
        public string? ExternalID { get; set; }

        [JsonPropertyName("externalIDFirm")]
        [Field(FieldName = "external_id_firm", Type = ParamType.String)]
        public string? ExternalIDFirm { get; set; }

        [JsonPropertyName("policy")]
        [Index(Name = IdxPolicy)]
        [Filter(Name = FilterPolicy)]
        [Field(FieldName = "policy", Type = ParamType.String)]
        public string? Policy { get; set; }

        [JsonPropertyName("description")]
        [Field(FieldName = "description", Type = ParamType.String)]
        public string? Description { get; set; }

        [JsonPropertyName("date")]
        [Index(Name = IdxDate)]
        [Filter(Name = FilterDate)]
        [Field(FieldName = "file_date", Type = ParamType.DateTime)]
        public DateTime Date { get; set; }

        [JsonPropertyName("status")]
        [Filter(Name = FilterStatus)]
        [Field(FieldName = "status", Type = ParamType.Int32, DefaultValue = EBoxAppointmentStatus.InProgress)]
        public EBoxFileStatus Status { get; set; }

        //Datos de la direccion de la intervencion
        [JsonPropertyName("address")]
        [Index(Name = IdxAddress)]
        [Index(Name = IdxAllAddressFields)]
        [Filter(Name = FilterAddress)]
        [Field(FieldName = "address", Type = ParamType.String, AllowNull = true)]
        public string? Address { get; set; }

        [JsonPropertyName("postalCode")]
        [Index(Name = IdxAllAddressFields)]
        [Filter(Name = FilterPostalCode)]
        [Field(FieldName = "postalcode", Type = ParamType.String, AllowNull = true)]
        public string? PostalCode { get; set; }

        [JsonPropertyName("province")]
        [Index(Name = IdxAllAddressFields)]
        [Filter(Name = FilterProvince)]
        [Field(FieldName = "province", Type = ParamType.String, AllowNull = true)]
        public string? Province { get; set; }

        [JsonPropertyName("population")]
        [Index(Name = IdxAllAddressFields)]
        [Filter(Name = FilterPopulation)]
        [Field(FieldName = "population", Type = ParamType.String, AllowNull = true)]
        public string? Population { get; set; }

        [JsonPropertyName("contactPerson1")]
        [Field(FieldName = "contact_person_1", Type = ParamType.String, AllowNull = true)]
        public string? ContactPerson1 { get; set; }

        [JsonPropertyName("phoneContact1")]
        [Field(FieldName = "phone_contact_1", Type = ParamType.String, AllowNull = true)]
        public string? PhoneContact1 { get; set; }

        [JsonPropertyName("contactPerson2")]
        [Field(FieldName = "contact_person_2", Type = ParamType.String, AllowNull = true)]
        public string? ContactPerson2 { get; set; }

        [JsonPropertyName("phoneContact2")]
        [Field(FieldName = "phone_contact_2", Type = ParamType.String, AllowNull = true)]
        public string? PhoneContact2 { get; set; }

        [JsonPropertyName("contactPerson3")]
        [Field(FieldName = "contact_person_3", Type = ParamType.String, AllowNull = true)]
        public string? ContactPerson3 { get; set; }

        [JsonPropertyName("phoneContact3")]
        [Field(FieldName = "phone_contact_3", Type = ParamType.String, AllowNull = true)]
        public string? PhoneContact3 { get; set; }

        [JsonPropertyName("urgent")]
        [Field(FieldName = "urgent", Type = ParamType.Boolean, DefaultValue = false)]
        public bool Urgent { get; set; }

        public string IDRecord => this.ID.ToString();

        internal DTOBoxFile CopyTo(DTOBoxFile f)
        {
            f.Address = this.Address;
            f.Date = this.Date;
            f.Description = this.Description;
            f.ID = this.ID;
            f.Code = this.Code;
            f.Number = this.Number;
            f.Policy = this.Policy;
            f.Population = this.Population;
            f.PostalCode = this.PostalCode;
            f.Province = this.Province;
            f.RefIntermediary = this.RefIntermediary;
            f.RefProvider = this.RefProvider;
            f.RefReceiver = this.RefReceiver;
            f.Status = this.Status;
            f.UUID = this.UUID;
            f.PhoneContact1 = this.PhoneContact1;
            f.PhoneContact2 = this.PhoneContact2;
            f.PhoneContact3 = this.PhoneContact3;
            f.Urgent = this.Urgent;
            if (this.Receiver != null)
            {
                f.Receiver = this.Receiver.CopyTo(new DTOBoxSubject());
            }
            else
            {
                f.Receiver = null;
            }
            if (this.Provider != null)
            {
                f.Provider = this.Provider.CopyTo(new DTOBoxSubject());
            }
            else
            {
                f.Provider = null;
            }
            if (this.Intermediary != null)
            {
                f.Intermediary = this.Intermediary.CopyTo(new DTOBoxSubject());
            }
            else
            {
                f.Intermediary = null;
            }
            if (this.AppointmentList != null)
            {
                f.AppointmentList = new List<DTOBoxAppointment>(this.AppointmentList.Count);
                foreach (var item in this.AppointmentList)
                {
                    f.AppointmentList.Add(item.CopyTo(new DTOBoxAppointment()));
                }
            }
            else
            {
                this.AppointmentList = null;
            }
            return f;
        }
    }
}
