using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "messages", FilePerTable = true)]
    public class DTOBoxMessage
    {
        public const string TAG = "DTOBoxMessage";
        public const string FilterID = TAG + "ID";
        public const string FilterRefFile = TAG + "RefFile";
        public const string FilterRefAppointment = TAG + "RefAppointment";
        public const string FilterRefSubject = TAG + "Subject";
        public const string FilterIdentifier = TAG + "Identifier";
        public const string IdxRefFile = TAG + "RefFile";
        public const string IdxRefAppointment = TAG + "RefAppointment";
        public const string IdxRefSubject = TAG + "Subject";
        public const string IdxIdentifier = TAG + "Identifier";        
        public const string IdxTypicalSearch_1 = TAG + "TypicalSearch_1";
        public const string IdxTypicalSearch_2 = TAG + "TypicalSearch_2";
        public const string IdxTypicalSearch_3 = TAG + "TypicalSearch_3";
        public const string IdxTypicalSearch_4 = TAG + "TypicalSearch_4";

        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [Filter(Name = FilterRefFile)]
        [Index(Name = IdxRefFile)]
        [Index(Name = IdxTypicalSearch_1)]
        [Index(Name = IdxTypicalSearch_3)]
        [Field(FieldName = "ref_file", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefFile { get; set; } = int.MinValue;

        [Index(Name = IdxTypicalSearch_2)]
        [Index(Name = IdxTypicalSearch_3)]
        [Filter(Name = FilterRefAppointment)]
        [Index(Name = IdxRefAppointment)]
        [Field(FieldName = "ref_appointment", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefAppointment { get; set; } = int.MinValue;

        [Index(Name = IdxTypicalSearch_3)]
        [Index(Name = IdxTypicalSearch_4)]
        [Filter(Name = FilterRefSubject)]
        [Field(FieldName = "ref_subject", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        internal int RefSubject { get; set; } = int.MinValue;

        [Filter(Name = FilterIdentifier)]
        [Index(Name = IdxIdentifier, Unique = true)]
        [Field(FieldName = "identifier", Type = ParamType.String)]
        public string Identifier { get; set; }

        [Field(FieldName = "date", Type = ParamType.DateTime)]
        public DateTime Date { get; set; }

        [Index(Name = IdxTypicalSearch_4)]
        [Index(Name = IdxTypicalSearch_3)]
        [Index(Name = IdxTypicalSearch_2)]
        [Index(Name = IdxTypicalSearch_1)]
        [Field(FieldName = "type_activity", Type = ParamType.Int32, DefaultValue = int.MinValue)]
        public EBoxMessageType Type { get; set; }

        [Field(FieldName = "activity", Type = ParamType.String)]
        public string? Message { get; set; }

        public string? VirtualMessage { get; set; }

        /// <summary>
        /// Si el tamaño de la actividad es extendido se guardara en un fichero de texto
        /// </summary>
        [Field(FieldName = "file", Type = ParamType.String)]
        public string? File { get; set; }

        [Field(FieldName = "flow", Type = ParamType.Int32, DefaultValue = (int)EBoxMessageFlow.In)]
        public EBoxMessageFlow Flow { get; set; }
    }
}
