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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public class ConjunctionPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.ConjunctionPhrase;

        /// <inheritdoc/>
        public override bool IsConjunctionPhrase => true;

        /// <inheritdoc/>
        public override ConjunctionPhrase AsConjunctionPhrase => this;

        /// <summary>
        /// The first conjuction in pair of correlative conjunctions.
        /// "both" in "both cat and dog"
        /// </summary>
        public Word PreConj { get; set; }

        /// <summary>
        /// "cat" in "both cat and dog"
        /// "cat" in "cat and dog"
        /// </summary>
        public BaseSentenceItem Left { get; set; }

        /// <summary>
        /// The root word of the sentence or second conjuction in pair of correlative conjunctions.
        /// "and" in "both cat and dog"
        /// "and" in "cat and dog"
        /// </summary>
        public Word Conj { get; set; }

        /// <summary>
        /// "dog" in "both cat and dog"
        /// "dog" in "cat and dog"
        /// </summary>
        public BaseSentenceItem Right { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => Conj.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => Conj.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return Conj.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public ConjunctionPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public ConjunctionPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ConjunctionPhrase)context[this];
            }

            var result = new ConjunctionPhrase();
            context[this] = result;

            result.PreConj = PreConj?.Clone(context);
            result.Left = Left?.CloneBaseSentenceItem(context);
            result.Conj = Conj?.Clone(context);
            result.Right = Right?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(PreConj), PreConj);
            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Conj), Conj);
            sb.PrintObjProp(n, nameof(Right), Right);


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
