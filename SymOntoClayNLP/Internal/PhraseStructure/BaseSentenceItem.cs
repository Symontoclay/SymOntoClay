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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public abstract class BaseSentenceItem : IObjectToString, IObjectToDbgString
    {
        public abstract KindOfSentenceItem KindOfSentenceItem { get; }

        public virtual bool IsSentence => false;
        public virtual Sentence AsSentence => null;
        
        public virtual bool IsNounPhrase => false;
        public virtual NounPhrase AsNounPhrase => null;

        public virtual bool IsVerbPhrase => false;
        public virtual VerbPhrase AsVerbPhrase => null;

        public virtual bool IsAdjectivePhrase => false;
        public virtual AdjectivePhrase AsAdjectivePhrase => null;

        public virtual bool IsPreOrPostpositionalPhrase => false;
        public virtual PreOrPostpositionalPhrase AsPreOrPostpositionalPhrase => null;

        public virtual bool IsAdverbialPhrase => false;
        public virtual AdverbialPhrase AsAdverbialPhrase => null;

        public virtual bool IsComplementizerPhrase => false;
        public virtual ComplementizerPhrase AsComplementizerPhrase => null;

        public virtual bool IsQuantifierPhrase => false;
        public virtual QuantifierPhrase AsQuantifierPhrase => null;

        public virtual bool IsConjunctionPhrase => false;
        public virtual ConjunctionPhrase AsConjunctionPhrase => null;

        public virtual bool IsListPhrase => false;
        public virtual ListPhrase AsListPhrase => null;

        public virtual bool IsWord => false;
        public virtual Word AsWord => null;

        public abstract BaseGrammaticalWordFrame RootWordFrame { get; }
        public abstract string RootWordAsString { get; }

        public abstract IList<string> GetConditionalLogicalMeaning(string conditionalWord);

        public string GetConditionalLogicalMeaningAsSingleString(string conditionalWord)
        {
            var list = GetConditionalLogicalMeaning(conditionalWord);

            if (list.Count > 1)
            {
                throw new NotImplementedException();
            }

            return list.FirstOrDefault();
        }

        public BaseSentenceItem CloneBaseSentenceItem()
        {
            var context = new Dictionary<object, object>();
            return CloneBaseSentenceItem(context);
        }

        public abstract BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context);

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            return PropertiesToDbgString(n);
        }

        protected abstract string PropertiesToDbgString(uint n);
    }
}
