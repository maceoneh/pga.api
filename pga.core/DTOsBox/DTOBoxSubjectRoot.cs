using es.dmoreno.utils.dataaccess.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "subjects_root")]
    internal class DTOBoxSubjectRoot
    {
        [Field(FieldName = "ref_subject", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        public int RefSubject { get; set; }

        [Field(FieldName = "user_md5", Type = ParamType.String)]
        public string UserMD5 { get; set; }

        [Field(FieldName = "password_md5", Type = ParamType.String)]
        public string PasswordMD5 { get; set; }
    }
}
