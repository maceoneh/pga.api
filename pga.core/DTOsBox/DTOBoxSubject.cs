using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace pga.core.DTOsBox
{    
    [Table(Name = "subjects")]
    public class DTOBoxSubject
    {
        /// <summary>
        /// Indica que solo contiene el valor UUID, este metodo sirve para saber si se quiere hacer una carga del dato
        /// buscando UUID, por ejemplo, cuando se quiere asociar un subject a un expediente y se necesita solo su id
        /// </summary>
        internal bool OnlyContainUUID
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UUID) &&
                        string.IsNullOrWhiteSpace(this.Name) &&
                        string.IsNullOrWhiteSpace(this.Surname) &&
                        string.IsNullOrWhiteSpace(this.Address) &&
                        string.IsNullOrWhiteSpace(this.Population) &&
                        string.IsNullOrWhiteSpace(this.Province) &&
                        string.IsNullOrWhiteSpace(this.eMail);
            }
        }

        public const string TAG = "DTOBoxSubject";
        public const string FilterID = TAG + "ID";
        public const string FilterUUID = TAG + "UUID";
        public const string FilterName = TAG + "Name";
        public const string FilterSurName = TAG + "SurName";
        public const string FilterAddress = TAG + "Address";
        public const string FilterPostalCode = TAG + "PostalCode";
        public const string FilterProvince = TAG + "Province";
        public const string FilterPopulation = TAG + "Population";
        public const string FilterEMail = TAG + "EMail";
        public const string IdxUUID = TAG + "UUID";
        public const string IdxAllPublicFields = TAG + "AllPublicFields";
        public const string IdxName = TAG + "Name";
        public const string IdxAddress = TAG + "Address";
        public const string IdxEmail = TAG + "EMail";

        [Filter(Name = FilterID)]
        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID  { get; set; }

        [JsonPropertyName("uuid")]
        [Index(Name = IdxUUID, Unique = true)]
        [Filter(Name = FilterUUID)]
        [Field(FieldName = "uuid", Type = ParamType.String)]
        public string? UUID { get; set; }

        [JsonPropertyName("name")]
        [Index(Name = IdxAllPublicFields)]
        [Index(Name = IdxName)]
        [Filter(Name = FilterName)]
        [Field(FieldName = "name", Type = ParamType.String)]
        public string? Name { get; set; }

        
        [Index(Name = IdxName)]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterSurName)]
        [Field(FieldName = "surname", Type = ParamType.String)]
        public string? Surname { get; set; }

        [JsonPropertyName("address")]
        [Index(Name = IdxAddress)]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterAddress)]
        [Field(FieldName = "address", Type = ParamType.String, AllowNull = true)]
        public string? Address { get; set; }

        [JsonPropertyName("postalCode")]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterPostalCode)]
        [Field(FieldName = "postalcode", Type = ParamType.String, AllowNull = true)]
        public string? PostalCode { get; set; }

        [JsonPropertyName("province")]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterProvince)]
        [Field(FieldName = "province", Type = ParamType.String, AllowNull = true)]
        public string? Province { get; set; }

        [JsonPropertyName("population")]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterPopulation)]
        [Field(FieldName = "population", Type = ParamType.String, AllowNull = true)]
        public string? Population { get; set; }

        [JsonPropertyName("eMail")]
        [Index(Name = IdxEmail)]
        [Index(Name = IdxAllPublicFields)]
        [Filter(Name = FilterEMail)]
        [Field(FieldName = "email", Type = ParamType.String, AllowNull = true)]
        public string? eMail { get; set; }
    }
}
