using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core
{
    internal class BoxDefinitions
    {
        /// <summary>
        /// Acción de crear un elemento en una entidad
        /// </summary>
        public const string ActionCreate = "create";

        /// <summary>
        /// Acción de modificar un elemento en una entidad
        /// </summary>
        public const string ActionModify = "modify";

        /// <summary>
        /// Acción de eliminar un elemento en una entidad
        /// </summary>
        public const string ActionDelete = "delete";

        /// <summary>
        /// Entidad que representa una cita
        /// </summary>
        public const string EntityAppointment = "appointment";

        /// <summary>
        /// Entidad que representa un sujeto
        /// </summary>
        public const string EntitySubject = "subject";

        /// <summary>
        /// Entidad que representa al grupo de sujetos que son ROOT del buzón
        /// </summary>
        public const string EntityAddSubjectRoot = "subject_root";

        /// <summary>
        /// Entidad que representa al grupo de sujetos que son empleados en el buzón
        /// </summary>
        public const string EntityAddSubjectEmploy = "subject_employ";

        /// <summary>
        /// Entidad que representa los expedientes almacenados en el buzon
        /// </summary>
        public const string EntityFile = "file";

        /// <summary>
        /// Entidad que representa los mensajes almacenados en el buzón
        /// </summary>
        public const string EntityMessage = "message";
    }
}
