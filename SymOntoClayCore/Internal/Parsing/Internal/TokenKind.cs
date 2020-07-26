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
        Channel,
        SystemVar,
        Point,
        Comma,
        /// <summary>
        /// Represents symbol `{`.
        /// </summary>
        OpenFigureBracket,

        /// <summary>
        /// Represents symbol `}`.
        /// </summary>
        CloseFigureBracket,
        /// <summary>
        /// Represents symbol `[`.
        /// </summary>
        OpenSquareBracket,
        /// <summary>
        /// Represents symbol `]`.
        /// </summary>
        CloseSquareBracket,
        /// <summary>
        /// Represents symbol `(`.
        /// </summary>
        OpenRoundBracket,
        /// <summary>
        /// Represents symbol `)`.
        /// </summary>
        CloseRoundBracket,
        /// <summary>
        /// Represents symbol `;`.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Represents symbol `=`.
        /// </summary>
        Assign,

        /// <summary>
        /// Represents symbol `=>`.
        /// </summary>
        Lambda,

        /// <summary>
        /// Represents symbol `&gt;`.
        /// </summary>
        More,

        /// <summary>
        /// Represents symbol `&gt;&gt;`.
        /// </summary>
        LeftRightStream
    }
}
