using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public enum TokenKind
    {
        /// <summary>
        /// Default value. Represents nothing.
        /// </summary>
        Unknown,
        Word,
        Number,
        String,
        Identifier,
        /// <summary>
        /// Represents symbol `{`.
        /// </summary>
        OpenFigureBracket,

        /// <summary>
        /// Represents symbol `}`.
        /// </summary>
        CloseFigureBracket,
        /// <summary>
        /// Represents symbol `;`.
        /// </summary>
        Semicolon,
    }
}
