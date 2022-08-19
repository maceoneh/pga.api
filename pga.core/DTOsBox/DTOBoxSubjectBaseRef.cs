using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace pga.core.DTOsBox
{
    internal abstract class DTOBoxSubjectBaseRef
    {
        public const string TAG = "DTOBoxSubjectBaseRef";
        public const string FilterRefSubject = TAG + "RefSubject";

        [Filter(Name = FilterRefSubject)]
        [Field(FieldName = "ref_subject", IsPrimaryKey = true, IsAutoincrement = false, Type = ParamType.Int32)]
        public int RefSubject { get; set; }
    }
}
