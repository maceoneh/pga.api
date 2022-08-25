using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace pga.core.DTOsBox
{
    [Table(Name = "files", FilePerTable = true)]
    public class DTOBoxFile
    {
        public const string TAG = "DTOBoxFile";
        public const string FilterID = TAG + "ID";
        public const string FilterUUID = TAG + "UUID";
        public const string FilterRefReceiver = TAG + "RefReceiver";
        public const string FilterRefProvider = TAG + "RefProvider";
        public const string FilterRefIntermediary = TAG + "RefIntermediary";
        public const string FilterPolicy = TAG + "Policy";
        public const string FilterDate = TAG + "Date";
        public const string IdxUUID = TAG + "UUID";
        public const string IdxRefReceiver = TAG + "RefInsured";
        public const string IdxRefProvider = TAG + "RefProvider";
        public const string IdxRefIntermediary = TAG + "RefIntermediary";
        public const string IdxPolicy = TAG + "Policy";
        public const string IdxDate = TAG + "Date";

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
    }
}
