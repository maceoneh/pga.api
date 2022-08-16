using es.dmoreno.utils.dataaccess.db;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    [Table(Name = "subjects")]
    public class DTOBoxSubject
    {
        //[Field]
        internal int ID  { get; set; }
        public string UUID { get; set; }

        public string Name { get; set; }
    }
}
