using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public enum NumeralType
    {
        Undefined,

        /// <summary>
        /// Such as `one`
        /// </summary>
        Cardinal,

        /// <summary>
        /// Such as `first`
        /// </summary>
        Ordinal
    }
}
