/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
    public class QuantifierPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.QuantifierPhrase;

        /// <inheritdoc/>
        public override bool IsQuantifierPhrase => true;

        /// <inheritdoc/>
        public override QuantifierPhrase AsQuantifierPhrase => this;

        /// <summary>
        /// The root word of the sentence.
        /// "many" in "very many"
        /// </summary>
        public BaseSentenceItem Q { get; set; }

        /// <summary>
        /// "very" in "very many"
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => Q.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => Q.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return Q.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public QuantifierPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public QuantifierPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (QuantifierPhrase)context[this];
            }

            var result = new QuantifierPhrase();
            context[this] = result;

            result.Q = Q?.CloneBaseSentenceItem(context);
            result.Adv = Adv?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Q), Q);
            sb.PrintObjProp(n, nameof(Adv), Adv);


            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            throw new NotImplementedException();

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}
