using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace pga.core.DTOsBox
{
    [Table(Name = "subjects")]
    public class DTOBoxSubject
    {
        public const string TAG = "DTOBoxSubject";
        public const string FilterUUID = TAG + "UUID";
        public const string IdxUUID = TAG + "UUID";

        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID  { get; set; }

        [Index(Name = IdxUUID, Unique = true)]
        [Filter(Name = FilterUUID)]
        [Field(FieldName = "uuid", Type = ParamType.String)]
        public string UUID { get; set; }

        [Field(FieldName = "name", Type = ParamType.String)]
        public string Name { get; set; }

        [Field(FieldName = "surname", Type = ParamType.String)]
        public string Surname { get; set; }

        [Field(FieldName = "address", Type = ParamType.String)]
        public string Address { get; set; }

        [Field(FieldName = "postalcode", Type = ParamType.String)]
        public string PostalCode { get; set; }

        [Field(FieldName = "province", Type = ParamType.String)]
        public string Province { get; set; }

        [Field(FieldName = "population", Type = ParamType.String)]
        public string Population { get; set; }

        [Field(FieldName = "email", Type = ParamType.String)]
        public string eMail { get; set; }
    }
}
