using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "subjects_root")]
    internal class DTOBoxSubjectRoot : DTOBoxSubjectBaseRef
    {
        /// <summary>
        /// Indica un usuario no valido el cual se aplica por defecto al crear un registro
        /// </summary>
        internal const string NoUsableUser = "default_user";

        /// <summary>
        /// Indica un password no valido el cual se aplica por defecto al crear un registro
        /// </summary>
        internal const string NoUsablePassword = "default_password";

        public const string TAG = "DTOBoxSubjectRoot";
        //public const string FilterRefSubject = TAG + "RefSubject";

        //[Filter(Name = FilterRefSubject)]
        //[Field(FieldName = "ref_subject", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        //public int RefSubject { get; set; }

        [Field(FieldName = "user_md5", Type = ParamType.String)]
        public string UserMD5 { get; set; }

        [Field(FieldName = "password_md5", Type = ParamType.String)]
        public string PasswordMD5 { get; set; }
    }
}
