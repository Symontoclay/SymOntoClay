﻿using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Noun_phrase
    /// NP
    /// </summary>
    public class NounPhrase: BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.NounPhrase;

        /// <inheritdoc/>
        public override bool IsNounPhrase => true;

        /// <inheritdoc/>
        public override NounPhrase AsNounPhrase => this;

        /// <summary>
        /// Noun, NounPhrase, ConjunctionPhrase, QuantifierPhrase which is root of the phrase.
        /// </summary>
        public BaseSentenceItem N { get; set; }

        /// <summary>
        /// Determiner
        /// Such as "the", "this", "my", "some", "Jane's"
        /// [the guy with a hat]'s dog
        /// [the girl who was laughing]'s scarf
        /// </summary>
        public BaseSentenceItem D { get; set; }

        /// <summary>
        /// "many" in "many very happy girls"
        /// </summary>
        public BaseSentenceItem QP { get; set; }

        /// <summary>
        /// such as "large", "beautiful", "sweeter"
        ///  such as "extremely large", "hard as nails", "made of wood", "sitting on the step"
        /// </summary>
        public BaseSentenceItem AP { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Noun_adjunct
        /// "college" in "a college student"
        /// </summary>
        public BaseSentenceItem NounAdjunct { get; set; }

        /// <summary>
        /// Represents "not"
        /// </summary>
        public Word Negation { get; set; }

        public bool HasPossesiveMark { get; set; }

        /// <summary>
        /// such as "in the drawing room", "of his aunt"
        /// </summary>
        public BaseSentenceItem PP { get; set; }

        /// <summary>
        /// such as "(over) there" in the noun phrase "the man (over) there"
        /// </summary>
        public BaseSentenceItem AdvP { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Relative_clause
        /// such as "which we noticed"
        /// </summary>
        public BaseSentenceItem RelativeClauses { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Clause
        /// other clauses serving as complements to the noun, such as "that God exists" in the noun phrase "the belief that God exists"
        /// </summary>
        public BaseSentenceItem OtherClauses { get; set; }

        /// <summary>
        /// infinitive phrases, such as "to sing well" and "to beat" in the noun phrases "a desire to sing well" and "the man to beat"
        /// </summary>
        public BaseSentenceItem InfinitivePhrases { get; set; }

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => N.RootWordFrame;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(N), N);
            sb.PrintObjProp(n, nameof(D), D);
            sb.PrintObjProp(n, nameof(QP), QP);
            sb.PrintObjProp(n, nameof(AP), AP);
            sb.PrintObjProp(n, nameof(NounAdjunct), NounAdjunct);
            sb.PrintObjProp(n, nameof(Negation), Negation);
            sb.AppendLine($"{spaces}{nameof(HasPossesiveMark)} = {HasPossesiveMark}");
            sb.PrintObjProp(n, nameof(PP), PP);
            sb.PrintObjProp(n, nameof(AdvP), AdvP);            
            sb.PrintObjProp(n, nameof(RelativeClauses), RelativeClauses);
            sb.PrintObjProp(n, nameof(OtherClauses), OtherClauses);
            sb.PrintObjProp(n, nameof(InfinitivePhrases), InfinitivePhrases);

            //sb.PrintObjProp(n, nameof(), );

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNspaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}NP");

            if(N != null)
            {
                sb.AppendLine($"{nextNspaces}N:");
                sb.Append(N.ToDbgString(nextNextN));
            }

            if (D != null)
            {
                sb.AppendLine($"{nextNspaces}D:");
                sb.Append(D.ToDbgString(nextNextN));
            }

            if (QP != null)
            {
                throw new NotImplementedException();
            }

            if (AP != null)
            {
                throw new NotImplementedException();
            }

            if (NounAdjunct != null)
            {
                throw new NotImplementedException();
            }

            if (Negation != null)
            {
                throw new NotImplementedException();
            }

            if (HasPossesiveMark)
            {
                throw new NotImplementedException();
            }

            if (PP != null)
            {
                throw new NotImplementedException();
            }

            if (AdvP != null)
            {
                throw new NotImplementedException();
            }

            if (RelativeClauses != null)
            {
                throw new NotImplementedException();
            }

            if (OtherClauses != null)
            {
                throw new NotImplementedException();
            }

            if (InfinitivePhrases != null)
            {
                throw new NotImplementedException();
            }

            return sb.ToString();
        }
    }
}
