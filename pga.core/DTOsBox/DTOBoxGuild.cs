using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "guilds")]
    public class DTOBoxGuild
    {
        public const string TAG = "DTOBoxGuild";
        public const string FilterID = TAG + "ID";
        public const string FilterName = TAG + "Name";

        [Filter(Name = FilterID)]
        [Field(FieldName = "id", IsAutoincrement = true, IsPrimaryKey = true, Type = ParamType.Int32)]
        public int ID { get; set; }

        [Filter(Name = FilterName)]
        [Field(FieldName = "name", Type = ParamType.String)]
        public string Name { get; set; }
    }
}
