using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
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
        public Word Negation { get; set; }

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

        public GrammaticalAspect Aspect { get; set; } = GrammaticalAspect.Undefined;
        public GrammaticalTenses Tense { get; set; } = GrammaticalTenses.Undefined;
        public GrammaticalVoice Voice { get; set; } = GrammaticalVoice.Undefined;
        public AbilityModality AbilityModality { get; set; } = AbilityModality.Undefined;
        public PermissionModality PermissionModality { get; set; } = PermissionModality.Undefined;
        public ObligationModality ObligationModality { get; set; } = ObligationModality.Undefined;
        public ProbabilityModality ProbabilityModality { get; set; } = ProbabilityModality.Undefined;
        public ConditionalModality ConditionalModality { get; set; } = ConditionalModality.Undefined;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Intrusion), Intrusion);
            sb.PrintObjProp(n, nameof(V), V);
            sb.PrintObjProp(n, nameof(VP), VP);
            sb.PrintObjProp(n, nameof(Negation), Negation);
            sb.PrintObjProp(n, nameof(Object), Object);
            sb.PrintObjListProp(n, nameof(PP), PP);
            sb.PrintObjProp(n, nameof(CP), CP);

            //sb.PrintObjProp(n, nameof(), );

            sb.AppendLine($"{spaces}{nameof(Aspect)} = {Aspect}");
            sb.AppendLine($"{spaces}{nameof(Tense)} = {Tense}");
            sb.AppendLine($"{spaces}{nameof(Voice)} = {Voice}");
            sb.AppendLine($"{spaces}{nameof(AbilityModality)} = {AbilityModality}");
            sb.AppendLine($"{spaces}{nameof(PermissionModality)} = {PermissionModality}");
            sb.AppendLine($"{spaces}{nameof(ObligationModality)} = {ObligationModality}");
            sb.AppendLine($"{spaces}{nameof(ProbabilityModality)} = {ProbabilityModality}");
            sb.AppendLine($"{spaces}{nameof(ConditionalModality)} = {ConditionalModality}");

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

            var sbMark = new StringBuilder();

            if (Aspect != GrammaticalAspect.Undefined ||
                Tense != GrammaticalTenses.Undefined || Voice != GrammaticalVoice.Undefined || AbilityModality != AbilityModality.Undefined ||
                PermissionModality != PermissionModality.Undefined || ObligationModality != ObligationModality.Undefined ||
                ProbabilityModality != ProbabilityModality.Undefined || ConditionalModality != ConditionalModality.Undefined)
            {
                sbMark.Append(":(");

                if (Aspect != GrammaticalAspect.Undefined)
                {
                    sbMark.Append($"{Aspect}");
                }

                if (Tense != GrammaticalTenses.Undefined)
                {
                    sbMark.Append($";{Tense}");
                }

                if (Voice != GrammaticalVoice.Undefined)
                {
                    sbMark.Append($";{Voice}");
                }

                if (AbilityModality != AbilityModality.Undefined)
                {
                    sbMark.Append($";{AbilityModality}");
                }

                if (PermissionModality != PermissionModality.Undefined)
                {
                    sbMark.Append($";{PermissionModality}");
                }

                if (ObligationModality != ObligationModality.Undefined)
                {
                    sbMark.Append($";{ObligationModality}");
                }

                if (ProbabilityModality != ProbabilityModality.Undefined)
                {
                    sbMark.Append($";{ProbabilityModality}");
                }

                if (ConditionalModality != ConditionalModality.Undefined)
                {
                    sbMark.Append($";{ConditionalModality}");
                }

                sbMark.Append(")");
            }

            sb.AppendLine($"{spaces}VP{sbMark}");

            if(Intrusion != null)
            {
                throw new NotImplementedException();
            }

            if (V != null)
            {
                sb.AppendLine($"{nextNspaces}V:");
                sb.Append(V.ToDbgString(nextNextN));
            }

            if (VP != null)
            {
                throw new NotImplementedException();
            }

            if (Negation != null)
            {
                throw new NotImplementedException();
            }

            if (Object != null)
            {
                sb.AppendLine($"{nextNspaces}Object:");
                sb.Append(Object.ToDbgString(nextNextN));
            }

            if (PP != null)
            {
                throw new NotImplementedException();
            }

            if (CP != null)
            {
                throw new NotImplementedException();
            }

            return sb.ToString();
        }
    }
}
