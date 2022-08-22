using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "sessions")]
    internal class DTOBoxSession
    {
        public const string TAG = "DTOBoxSession";
        public const string FilterToken = TAG + "Token";
        public const string FilterApplicationKey = TAG + "ApplicationKey";

        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        internal int ID { get; set; }

        [Field(FieldName = "ref_subject", Type = ParamType.Int32)]
        internal int RefSubject { get; set; } = int.MinValue;

        [Filter(Name = FilterToken)]
        [Field(FieldName = "token", Type = ParamType.String)]
        public string Token { get; set; }

        /// <summary>
        /// Momento en el que la sesion se crea por primera vez
        /// </summary>
        [Field(FieldName = "creation_time", Type = ParamType.DateTime)]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Momento en el que la sesion se refresca para poder seguir usandose
        /// </summary>
        [Field(FieldName = "refresh_time", Type = ParamType.DateTime)]
        public DateTime RefreshTime { get; set; }

        [Field(FieldName = "ttl", Type = ParamType.Int32)]
        public int TTL { get; set; }

        /// <summary>
        /// Indica una key que representa un tipo de aplicación
        /// </summary>
        [Filter(Name = FilterApplicationKey)]
        [Field(FieldName = "application_key", Type = ParamType.String)]
        public string ApplicationKey { get; set; }
    }
}
