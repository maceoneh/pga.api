using es.dmoreno.utils.dataaccess.db;
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
        public const string IdxUserMD5 = TAG + "UserMD5";

        [Field(FieldName = "id", IsPrimaryKey = true, Type = ParamType.Int32, IsAutoincrement = true)]
        internal int ID { get; set; }

        [DataMember(Name = "user")]
        [Index(Name = IdxUserMD5, Unique = true)]
        [Field(FieldName = "user_md5", Type = ParamType.String)]
        public string UserMD5 { get; set; }

        [DataMember(Name = "password")]
        [Field(FieldName = "password_md5", Type = ParamType.String)]
        public string PasswordMD5 { get; set; }
    }
}
