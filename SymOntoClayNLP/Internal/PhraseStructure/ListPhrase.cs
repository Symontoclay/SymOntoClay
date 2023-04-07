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
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public class ListPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.ListPhrase;

        /// <inheritdoc/>
        public override bool IsListPhrase => true;

        /// <inheritdoc/>
        public override ListPhrase AsListPhrase => this;

        public List<BaseSentenceItem> Items { get; set; } = new List<BaseSentenceItem>();

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => throw new NotImplementedException();

        /// <inheritdoc/>
        public override string RootWordAsString => throw new NotImplementedException();

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            throw new NotImplementedException();
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
        public ListPhrase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public ListPhrase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ListPhrase)context[this];
            }

            var result = new ListPhrase();
            context[this] = result;

            result.Items = Items?.Select(p => p?.CloneBaseSentenceItem(context)).ToList();

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Items), Items);


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
