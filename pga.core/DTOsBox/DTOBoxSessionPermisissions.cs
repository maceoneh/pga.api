﻿using es.dmoreno.utils.permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    internal class DTOBoxSessionPermisissions
    {
        public string BoxIdentifier { get; set; }
        public List<DTOBoxSessionPermissionsByIdentifier> PermissionsBySession { get; set; } = new List<DTOBoxSessionPermissionsByIdentifier>();
    }

    internal class DTOBoxSessionPermissionsByIdentifier
    { 
        public string Session { get; set; }
        public bool IsRoot { get; set; }
        public List<DTOPermission?> Permissions { get; set; }
    }
}
