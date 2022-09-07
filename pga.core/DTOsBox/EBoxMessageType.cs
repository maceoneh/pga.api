using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    /// <summary>
    /// Indica el tipo de actividad ocurrida
    /// </summary>
    public enum EBoxMessageType
    {
        CreateFile = 0,
        DeleteFile = 1,
        ModifyFile = 2,
        CreateAppointment = 3,
        DeleteAppointment = 4,
        ModifyAppointment = 5,
        DownloadedAppointment = 6
    }
}
