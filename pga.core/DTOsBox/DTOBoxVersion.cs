using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace pga.core.DTOsBox
{
    [DataContract]
    internal class DTOBoxVersion
    {
        [DataMember(Name = "versionToken")]
        public string VersionToken { get; set; }

        [DataMember(Name = "lastUpdateDatabase")]
        public DateTime LastUpdateDatabase { get; set; }
    }
}
