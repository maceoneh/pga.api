using es.dmoreno.utils.dataaccess.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOs
{
    [Table(Name = "users", FilePerTable = true)]
    internal class DTOUser
    {
        public const string TAG = "DTOUser";
        public const string IdxUserMD5 = TAG + "UserMD5";

        [Field(FieldName = "id", IsPrimaryKey = true, Type = ParamType.Int32, IsAutoincrement = true)]
        internal int ID { get; set; }

        [Index(Name = IdxUserMD5, Unique = true)]
        [Field(FieldName = "user_md5", Type = ParamType.String)]
        public string UserMD5 { get; set; }

        [Field(FieldName = "password_md5", Type = ParamType.String)]
        public string PasswordMD5 { get; set; }
    }
}
