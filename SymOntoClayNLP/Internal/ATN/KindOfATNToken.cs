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
