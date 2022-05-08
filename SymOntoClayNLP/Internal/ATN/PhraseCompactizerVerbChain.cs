using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class PhraseCompactizerVerbChain : IObjectToString
    {
        public enum StateOfChain
        {
            Init,
            V
        }

        public StateOfChain State { get; set; } = StateOfChain.Init;

        public VerbPhrase V { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(State)} = {State}");

            sb.PrintObjProp(n, nameof(V), V);

            return sb.ToString();
        }
    }
}
