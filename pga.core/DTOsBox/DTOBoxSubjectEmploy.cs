using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "subject_employees")]
    public class DTOBoxSubjectEmploy : DTOBoxSubjectBaseRef
    {
        public const string TAG = "DTOBoxSubjectEmploy";
        public const string FilterUserPGAMobile = TAG + "UserPGAMobile";
        //public const string FilterRefSubject = TAG + "RefSubject";
        public const string IdxUniquePGAMobile = TAG + "UserPGAMobile";
        
        //[Filter(Name = FilterRefSubject)]
        //[Field(FieldName = "ref_subject", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        //public int RefSubject { get; set; }

        [Index(Name = IdxUniquePGAMobile, Unique = true)]
        [Filter(Name = FilterUserPGAMobile)]
        [Field(FieldName = "user_pgamobile", Type = ParamType.String)]
        public string UserPGAMobile { get; set; }
    }


}
