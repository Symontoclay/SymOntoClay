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
    public class AdverbialPhrase: BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.AdverbialPhrase;

        /// <inheritdoc/>
        public override bool IsAdverbialPhrase => true;

        /// <inheritdoc/>
        public override AdverbialPhrase AsAdverbialPhrase => this;

        /// <summary>
        /// The root adverbial of the phrase.
        /// </summary>
        public BaseSentenceItem Adv { get; set; }

        /// <summary>
        /// "quickly" in "very quickly"
        /// </summary>
        public BaseSentenceItem AdvP { get; set; }

        /// <summary>
        /// "beautiful" in "very beautiful"
        /// </summary>
        public BaseSentenceItem AP { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => Adv.RootWordFrame;

        /// <inheritdoc/>
        public override string RootWordAsString => Adv.RootWordAsString;

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            return Adv.GetConditionalLogicalMeaning(conditionalWord);
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public AdverbialPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public AdverbialPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AdverbialPhrase)context[this];
            }

            var result = new AdverbialPhrase();
            context[this] = result;

            result.Adv = Adv?.CloneBaseSentenceItem(context);
            result.AdvP = AdvP?.CloneBaseSentenceItem(context);
            result.AP = AP?.CloneBaseSentenceItem(context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Adv), Adv);
            sb.PrintObjProp(n, nameof(AdvP), AdvP);
            sb.PrintObjProp(n, nameof(AP), AP);


            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            throw new NotImplementedException("137A48A8-BA72-468A-89AF-062F929E3798");

            //var spaces = DisplayHelper.Spaces(n);
            //var sb = new StringBuilder();

            //return sb.ToString();
        }
    }
}
