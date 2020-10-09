/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
