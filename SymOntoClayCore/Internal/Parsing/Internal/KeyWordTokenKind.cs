using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public enum KeyWordTokenKind
    {
        /// <summary>
        /// Default value. Represents nothing.
        /// </summary>
        Unknown,
        World,
        Host,
        App,
        Class,
        Is,
        On,
        Init,
        Use,
        Not,
        Select,
        Insert
    }
}
