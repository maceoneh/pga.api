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
        //Tipos de mensajes que puede enviar PGAMobile
        PGAMobileText = 0,
        PGAMobileLocation = 1,
        PGAMobileImage = 2,
        PGAMobileVehicle = 3,
        PGAMobileInjured = 4,
        PGAMobileScales = 5,
        PGAMobileEmployee = 6,
        PGAMobileWorkDone = 7,
        PGAMobileAppointmentChange = 8,
        PGAMobileCheckIn = 9,
        PGAMobileBudget = 10,
        PGAMobileResolutionTree = 11,
        PGAMobileMapfreMilestone = 12,
        PGAMobileSendReceivedStatus = 997,
        PGAMobileOpeningAppointment = 998,
        PGAMobileCloseAppointment = 999,

        //Para mantener compatibilidad con PGA-Software 3 se comienza a partir del 1000
        CreateFile = 1000,
        DeleteFile = 1001,
        ModifyFile = 1002,
        CreateAppointment = 1003,
        DeleteAppointment = 1004,
        ModifyAppointment = 1005,
        DownloadedAppointment = 1006
    }
}
