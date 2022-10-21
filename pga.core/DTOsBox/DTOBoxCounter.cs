using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "counters")]
    internal class DTOBoxCounter
    {
        public const string TAG = "DTOBoxCounter";
        public const string FilterType = TAG + "Type";
        public const string FilterPrefix = TAG + "Prefix";

        [Filter(Name = FilterType)]
        [Field(FieldName = "type", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        public EBoxCounterType Type { get; set; }

        [Filter(Name = FilterPrefix)]
        [Field(FieldName = "prefix", Type = ParamType.String, DefaultValue = "")]
        public string Prefix { get; set; } = "";

        [Field(FieldName = "counter", Type = ParamType.Int32)]
        public int Counter { get; set; }        
    }
}
