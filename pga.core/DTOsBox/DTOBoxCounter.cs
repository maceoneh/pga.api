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

        [Filter(Name = FilterType)]
        [Field(FieldName = "type", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        public EBoxCounterType Type { get; set; }

        [Field(FieldName = "counter", Type = ParamType.Int32)]
        public int Counter { get; set; }
    }
}
