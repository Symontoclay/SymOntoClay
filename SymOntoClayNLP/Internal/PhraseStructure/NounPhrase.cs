using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    //https://en.wikipedia.org/wiki/Noun_phrase
    //NP
    public class NounPhrase: BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.NounPhrase;

        /// <inheritdoc/>
        public override bool IsNounPhrase => true;

        /// <inheritdoc/>
        public override NounPhrase AsNounPhrase => this;

        public Word Noun { get; set; }

        /// <summary>
        /// Such as "the", "this", "my", "some", "Jane's"
        /// </summary>
        public BaseSentenceItem Determiner { get; set; }

        /// <summary>
        /// [the guy with a hat]'s dog
        /// [the girl who was laughing]'s scarf
        /// </summary>
        public BaseSentenceItem DeterminerPhrase { get; set; }

        /// <summary>
        ///  such as "large", "beautiful", "sweeter"
        /// </summary>
        public Word Adjective { get; set; }

        /// <summary>
        ///  such as "extremely large", "hard as nails", "made of wood", "sitting on the step"
        /// </summary>
        public BaseSentenceItem AdjectivePhrase { get; set; }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Noun_adjunct
        /// "college" in "a college student"
        /// </summary>
        public Word NounAdjunct { get; set; }

        public BaseSentenceItem NounAdjunctPhrase { get; set; }

        /// <summary>
        /// such as "in the drawing room", "of his aunt"
        /// </summary>
        public PreOrPostpositionalPhrase PreOrPostpositionalPhrase { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Noun), Noun);
            sb.PrintObjProp(n, nameof(Determiner), Determiner);
            sb.PrintObjProp(n, nameof(DeterminerPhrase), DeterminerPhrase);
            sb.PrintObjProp(n, nameof(Adjective), Adjective);
            sb.PrintObjProp(n, nameof(AdjectivePhrase), AdjectivePhrase);
            sb.PrintObjProp(n, nameof(NounAdjunct), NounAdjunct);
            sb.PrintObjProp(n, nameof(NounAdjunctPhrase), NounAdjunctPhrase);
            sb.PrintObjProp(n, nameof(PreOrPostpositionalPhrase), PreOrPostpositionalPhrase);

            //sb.PrintObjProp(n, nameof(), );

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            throw new NotImplementedException();

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToDbgString(n));

            return sb.ToString();
        }
    }
}
