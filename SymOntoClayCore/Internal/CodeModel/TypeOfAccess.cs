using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum TypeOfAccess
    {
        Unknown,
        /// <summary>
        /// Defined in body of function.
        /// </summary>
        Local,
        /// <summary>
        /// Reserved for CodeEntities which will be accessible in module.
        /// </summary>
        Internal,
        Private,
        Protected,
        Public
    }
}
