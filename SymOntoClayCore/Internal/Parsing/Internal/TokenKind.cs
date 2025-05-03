/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        EntityCondition,
        OnceEntityCondition,
        Entity,
        Point,
        Comma,
        Plus,
        Minus,
        /// <summary>
        /// Represents symbol `*`.
        /// </summary>
        Multiplication,
        Division,
        QuestionMark,

        /// <summary>
        /// Represents symbol `~`.
        /// </summary>
        AsyncMarker,

        /// <summary>
        /// Represents symbol `~~`.
        /// </summary>
        DoubleAsyncMarker,

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
        /// 
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
        /// Represents symbol `[:`.
        /// </summary>
        OpenAnnotationBracket,

        /// <summary>
        /// Represents symbol `:]`.
        /// </summary>
        CloseAnnotationBracket,

        /// <summary>
        /// Represents symbol `:`.
        /// </summary>
        Colon,
        /// <summary>
        /// Represents symbol `::`.
        /// </summary>
        DoubleColon,
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
        MoreOrEqual,
        Less,
        LessOrEqual,

        /// <summary>
        /// Represents symbol `&amp;`.
        /// </summary>
        And,
        
        /// <summary>
        /// Represents symbol `|`.
        /// </summary>
        Or,

        /// <summary>
        /// Represents symbol `!`.
        /// </summary>
        Not,

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
        LeftRightArrow,

        /// <summary>
        /// Represents symbol `+∞`.
        /// </summary>
        PositiveInfinity,

        /// <summary>
        /// Represents symbol `-∞`.
        /// </summary>
        NegativeInfinity,
        Gravis,
        /// <summary>
        /// Represents symbol `#`.
        /// </summary>
        IdentifierPrefix,
        /// <summary>
        /// Represents symbol `#@`.
        /// </summary>
        EntityConditionPrefix,
        /// <summary>
        /// Represents symbol `##`.
        /// </summary>
        ConceptPrefix,
        /// <summary>
        /// Represents symbol `##@`.
        /// </summary>
        OnceResolvedEntityConditionPrefix,
        /// <summary>
        /// Represents symbol `#^`.
        /// </summary>
        RuleOrFactIdentifierPrefix,
        /// <summary>
        /// Represents symbol `#|`.
        /// </summary>
        LinguisticVarPrefix,
        /// <summary>
        /// Represents symbol `$`.
        /// </summary>
        LogicalVarPrefix,
        /// <summary>
        /// Represents symbol `@`.
        /// </summary>
        VarPrefix,
        /// <summary>
        /// Represents symbol `@@`.
        /// </summary>
        SystemVarPrefix,
        /// <summary>
        /// Represents symbol `@>`.
        /// </summary>
        ChannelVarPrefix,
        /// <summary>
        /// Represents symbol `@:`.
        /// </summary>
        PropertyPrefix
    }
}
