using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Verb_phrase
    /// VP
    /// see also https://theclassicjournal.uga.edu/index.php/2020/10/29/under-the-surface/
    /// </summary>
    public class VerbPhrase : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.VerbPhrase;

        /// <inheritdoc/>
        public override bool IsVerbPhrase => true;

        /// <inheritdoc/>
        public override VerbPhrase AsVerbPhrase => this;

        /// <summary>
        /// https://en.wikipedia.org/wiki/Constituent_(linguistics)
        /// "definitely" in "Drunks definitely could put off the customers", "Drunks could definitely put off the customers", "Drunks could put definitely off the customers" or "Drunks could put off definitely the customers".
        /// </summary>
        public BaseSentenceItem Intrusion { get; set; }

        public Word V { get; set; }

        public VerbPhrase VP { get; set; }

        /// <summary>
        /// Represents "not"
        /// </summary>
        public bool Negation { get; set; }

        /// <summary>
        /// Particle which modifies main verb.
        /// "off" in "put off", "on" in "turn on"
        /// </summary>
        public Word Particle { get; set; }

        /// <summary>
        /// In general it is NP
        /// </summary>
        public BaseSentenceItem Object { get; set; }

        /// <summary>
        /// "to school" in "I go to school tomorrow"
        /// </summary>
        public List<PreOrPostpositionalPhrase> PP { get; set; }

        /// <summary>
        /// "that she is a teacher" in "I know that she is a teacher"
        /// </summary>
        public BaseSentenceItem CP { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Intrusion), Intrusion);
            sb.PrintObjProp(n, nameof(V), V);
            sb.PrintObjProp(n, nameof(VP), VP);
            sb.AppendLine($"{spaces}{nameof(Negation)} = {Negation}");
            sb.PrintObjProp(n, nameof(Object), Object);
            sb.PrintObjListProp(n, nameof(PP), PP);
            sb.PrintObjProp(n, nameof(CP), CP);

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
