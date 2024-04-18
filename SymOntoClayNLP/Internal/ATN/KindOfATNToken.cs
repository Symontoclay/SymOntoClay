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

namespace SymOntoClay.NLP.Internal.ATN
{
    public enum KindOfATNToken
    {
        /// <summary>
        /// Default value. Represents nothing.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents some word.
        /// </summary>
        Word,

        /// <summary>
        /// Represents some number.
        /// </summary>
        Number,

        /// <summary>
        /// Represents symbol `(`.
        /// </summary>
        OpenRoundBracket,

        /// <summary>
        /// Represents symbol `)`.
        /// </summary>
        CloseRoundBracket,

        /// <summary>
        /// Represents symbol `,`.
        /// </summary>
        Comma,

        /// <summary>
        /// Represents symbol `:`.
        /// </summary>
        Colon,

        /// <summary>
        /// Represents symbol `.`.
        /// </summary>
        Point,

        /// <summary>
        /// Represents symbol `-`.
        /// </summary>
        Dash,

        /// <summary>
        /// Represents symbol `;`.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Represents symbol `!`.
        /// </summary>
        ExclamationMark,

        /// <summary>
        /// Represents symbol `?`.
        /// </summary>
        QuestionMark,

        /// <summary>
        /// Represents symbol `'`.
        /// </summary>
        SingleQuotationMark,

        /// <summary>
        /// Represents symbol `"`.
        /// </summary>
        DoubleQuotationMark
    }
}
