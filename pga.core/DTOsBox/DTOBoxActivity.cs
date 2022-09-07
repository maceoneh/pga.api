using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "activity", FilePerTable = true)]
    public class DTOBoxActivity
    {
        public const string TAG = "DTOBoxActivity";
        public const string FilterID = TAG + "ID";
        public const string FilterRefFile = TAG + "RefFile";
        public const string FilterRefAppointment = TAG + "RefAppointment";
        public const string FilterIdentifier = TAG + "Identifier";
        public const string IdxRefFile = TAG + "RefFile";
        public const string IdxRefAppointment = TAG + "RefAppointment";
        public const string IdxIdentifier = TAG + "Identifier";

        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [Filter(Name = FilterRefFile)]
        [Index(Name = IdxRefFile)]
        [Field(FieldName = "ref_file", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefFile { get; set; } = int.MinValue;

        [Filter(Name = FilterRefAppointment)]
        [Index(Name = IdxRefAppointment)]
        [Field(FieldName = "ref_appointment", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefAppointment { get; set; } = int.MinValue;

        [Filter(Name = FilterIdentifier)]
        [Index(Name = IdxIdentifier, Unique = true)]
        [Field(FieldName = "identifier", Type = ParamType.String)]
        public string Identifier { get; set; }

        [Field(FieldName = "date", Type = ParamType.DateTime)]
        public DateTime Date { get; set; }

        [Field(FieldName = "type_activity", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public EBoxActivityType Type { get; set; }

        [Field(FieldName = "activity", Type = ParamType.String)]
        public string? Activity { get; set; }

        public string? VirtualActivity { get; set; }

        /// <summary>
        /// Si el tamaño de la actividad es extendido se guardara en un fichero de texto
        /// </summary>
        [Field(FieldName = "file", Type = ParamType.String)]
        public string File { get; set; }

        [Field(FieldName = "flow", Type = ParamType.Int32, DefaultValue = (int)EBoxActivityFlow.In)]
        public EBoxActivityFlow Flow { get; set; }
    }
}
