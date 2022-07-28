using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace pga.core.DTOs
{
    [DataContract]
    [Table(Name = "users", FilePerTable = true)]
    public class DTOUser
    {
        public const string TAG = "DTOUser";
        public const string FilterUserMD5 = TAG + "UserMD5";
        public const string FilterID = TAG + "ID";
        public const string IdxUserMD5 = TAG + "UserMD5";

        [Filter(Name = FilterID)]
        [Field(FieldName = "id", IsPrimaryKey = true, Type = ParamType.Int32, IsAutoincrement = true)]
        internal int ID { get; set; }

        [DataMember(Name = "user")]
        [Filter(Name = FilterUserMD5)]
        [Index(Name = IdxUserMD5, Unique = true)]
        [Field(FieldName = "user_md5", Type = ParamType.String)]
        public string UserMD5 { get; set; }

        [DataMember(Name = "password")]
        [Field(FieldName = "password_md5", Type = ParamType.String)]
        public string PasswordMD5 { get; set; }
    }
}
