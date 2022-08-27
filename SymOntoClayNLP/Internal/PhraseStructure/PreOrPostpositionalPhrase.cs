/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://en.wikipedia.org/wiki/Adpositional_phrase
    //PP
    public class PreOrPostpositionalPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.PreOrPostpositionalPhrase;

        /// <inheritdoc/>
        public override bool IsPreOrPostpositionalPhrase => true;

        /// <inheritdoc/>
        public override PreOrPostpositionalPhrase AsPreOrPostpositionalPhrase => this;

        /// <summary>
        /// The root word of the phrase: "with" in "with them".
        /// Or "from" in "from now on".
        /// </summary>
        public BaseSentenceItem P { get; set; }

        /// <summary>
        /// "on" in "from now on".
        /// </summary>
        public BaseSentenceItem P2 { get; set; }

        /// <summary>
        /// "now" in "from now on".
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <summary>
        /// "them" in "with them"
        /// "two years" in "two years ago"
        /// </summary>
        public BaseSentenceItem NP { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => P.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => P.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return P.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public PreOrPostpositionalPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public PreOrPostpositionalPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PreOrPostpositionalPhrase)context[this];
            }

            var result = new PreOrPostpositionalPhrase();
            context[this] = result;

            result.P = P?.CloneBaseSentenceItem(context);
            result.P2 = P2?.CloneBaseSentenceItem(context);
            result.Adv = Adv?.CloneBaseSentenceItem(context);
            result.NP = NP?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(P), P);
            sb.PrintObjProp(n, nameof(P2), P2);
            sb.PrintObjProp(n, nameof(Adv), Adv);
            sb.PrintObjProp(n, nameof(NP), NP);

            //sb.PrintObjProp(n, nameof(), );

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNspaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}PP");

            if(P != null)
            {
                sb.AppendLine($"{nextNspaces}:P");
                sb.Append(P.ToDbgString(nextNextN));
            }

            if (P2 != null)
            {
                sb.AppendLine($"{nextNspaces}:P2");
                sb.Append(P2.ToDbgString(nextNextN));
            }

            if (Adv != null)
            {
                sb.AppendLine($"{nextNspaces}:Adv");
                sb.Append(Adv.ToDbgString(nextNextN));
            }

            if (NP != null)
            {
                sb.AppendLine($"{nextNspaces}:NP");
                sb.Append(NP.ToDbgString(nextNextN));
            }

            return sb.ToString();
        }
    }
}
