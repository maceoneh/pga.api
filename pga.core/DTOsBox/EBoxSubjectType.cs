using System;
using System.Collections.Generic;
using System.Text;

namespace pga.core.DTOsBox
{
    /// <summary>
    /// Indica el tipo de sujeto
    /// </summary>
    public enum EBoxSubjectType
    {
        /// <summary>
        /// El sujeto puede realizar operaciones de Root
        /// </summary>
        Root = 0,

        /// <summary>
        /// El sujeto puede realizar operaciones derivadas a un operario
        /// </summary>
        Employ = 1
    }
}
