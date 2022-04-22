using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class ATNToken : IObjectToString
    {
        public KindOfATNToken Kind { get; set; } = KindOfATNToken.Unknown;
        public string Content { get; set; } = string.Empty;
        public int Pos { get; set; }
        public int Line { get; set; }
        public IList<BaseGrammaticalWordFrame> WordFrames { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Content)} = {Content}");
            sb.AppendLine($"{spaces}{nameof(Pos)} = {Pos}");
            sb.AppendLine($"{spaces}{nameof(Line)} = {Line}");
            sb.PrintObjListProp(n, nameof(WordFrames), WordFrames);

            return sb.ToString();
        }
    }
}
