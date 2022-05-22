using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
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
