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
        Var,
        SystemVar,
        LogicalVar,
        QuestionVar,
        EntityCondition,
        Entity,
        Point,
        Comma,
        QuestionMark,
        /// <summary>
        /// Represents symbol `~`.
        /// </summary>
        AsyncMarker,
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
        /// Represents symbol `{:`.
        /// </summary>
        OpenFactBracket,
        /// <summary>
        /// Represents symbol `:}`.
        /// </summary>
        CloseFactBracket,
        /// <summary>
        /// Represents symbol `:`.
        /// </summary>
        Colon,
        /// <summary>
        /// Represents symbol `;`.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Represents symbol `=`.
        /// </summary>
        Assign,

        /// <summary>
        /// Represents symbol `=&gt;`.
        /// </summary>
        Lambda,

        /// <summary>
        /// Represents symbol `&gt;`.
        /// </summary>
        More,

        /// <summary>
        /// Represents symbol `&amp;`.
        /// </summary>
        And,

        /// <summary>
        /// Represents symbol `&gt;&gt;`.
        /// </summary>
        LeftRightStream,

        /// <summary>
        /// Represents symbol `&gt;:`.
        /// </summary>
        PrimaryLogicalPartMark,

        /// <summary>
        /// Represents symbol `-&gt;`.
        /// </summary>
        LeftRightArrow
    }
}
