using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "employees_in_appointment", FilePerTable = true)]
    public class DTOBoxEmployInAppointment
    {
        public const string TAG = "DTOBoxEmployInAppointment";
        internal const string FilterRefAppointment = TAG + "RefAppointment";
        internal const string IdxRefAppointment = TAG + "RefAppointment";

        [Index(Name = IdxRefAppointment)]
        [Filter(Name = FilterRefAppointment)]
        [Field(FieldName = "ref_appointment", IsAutoincrement = false, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int RefAppointment { get; set; } = int.MinValue;

        [Field(FieldName = "ref_employ", IsAutoincrement = false, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int RefEmploy { get; set; } = int.MinValue;
        public DTOBoxSubject? Employ { get; set; } = null;

        [Field(FieldName = "leading", Type = ParamType.Boolean, DefaultValue = false)]
        public bool Leading { get; set; } = false;
    }
}
