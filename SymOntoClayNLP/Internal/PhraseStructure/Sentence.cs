using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Phrase_structure_grammar
    /// https://en.wikipedia.org/wiki/Phrase_structure_rules
    /// https://en.wikipedia.org/wiki/Phrase
    /// </summary>
    public class Sentence : BaseSentenceItem
    {
        /// <inheritdoc/>
        public override KindOfSentenceItem KindOfSentenceItem => KindOfSentenceItem.Sentence;

        /// <inheritdoc/>
        public override bool IsSentence => true;

        /// <inheritdoc/>
        public override Sentence AsSentence => this;

        public BaseSentenceItem VocativePhrase { get; set; }
        public BaseSentenceItem Condition { get; set; }
        public BaseSentenceItem Subject { get; set; }
        public BaseSentenceItem Predicate { get; set; }        

        public GrammaticalMood Mood { get; set; } = GrammaticalMood.Undefined;

        public KindOfQuestion KindOfQuestion { get; set; } = KindOfQuestion.Undefined;
        public bool IsNegation { get; set; }

        public GrammaticalAspect Aspect { get; set; } = GrammaticalAspect.Undefined;
        public GrammaticalTenses Tense { get; set; } = GrammaticalTenses.Undefined;
        public GrammaticalVoice Voice { get; set; } = GrammaticalVoice.Undefined;
        public AbilityModality AbilityModality { get; set; } = AbilityModality.Undefined;
        public PermissionModality PermissionModality { get; set; } = PermissionModality.Undefined;
        public ObligationModality ObligationModality { get; set; } = ObligationModality.Undefined;
        public ProbabilityModality ProbabilityModality { get; set; } = ProbabilityModality.Undefined;
        public ConditionalModality ConditionalModality { get; set; } = ConditionalModality.Undefined;

        /// <inheritdoc/>
        public override BaseGrammaticalWordFrame RootWordFrame => throw new NotImplementedException();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(VocativePhrase), VocativePhrase);
            sb.PrintObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(Subject), Subject);
            sb.PrintObjProp(n, nameof(Predicate), Predicate);            

            //sb.PrintObjProp(n, nameof(), );

            sb.AppendLine($"{spaces}{nameof(Mood)} = {Mood}");

            sb.AppendLine($"{spaces}{nameof(KindOfQuestion)} = {KindOfQuestion}");
            sb.AppendLine($"{spaces}{nameof(IsNegation)} = {IsNegation}");

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

            if(Mood != GrammaticalMood.Undefined || (KindOfQuestion != KindOfQuestion.Undefined && KindOfQuestion != KindOfQuestion.None) || IsNegation || Aspect != GrammaticalAspect.Undefined ||
                Tense != GrammaticalTenses.Undefined || Voice != GrammaticalVoice.Undefined || (AbilityModality != AbilityModality.Undefined && AbilityModality != AbilityModality.None) ||
                (PermissionModality != PermissionModality.Undefined && PermissionModality != PermissionModality.None) || (ObligationModality != ObligationModality.Undefined && ObligationModality != ObligationModality.None) ||
                (ProbabilityModality != ProbabilityModality.Undefined && ProbabilityModality != ProbabilityModality.None) || (ConditionalModality != ConditionalModality.Undefined && ConditionalModality != ConditionalModality.None))
            {
                sbMark.Append(":(");

                if (Mood != GrammaticalMood.Undefined)
                {
                    sbMark.Append(Mood);
                }

                if (KindOfQuestion != KindOfQuestion.Undefined && KindOfQuestion != KindOfQuestion.None)
                {
                    sbMark.Append($";{KindOfQuestion}");
                }

                if (IsNegation)
                {
                    sbMark.Append($";{IsNegation}");
                }

                if (Aspect != GrammaticalAspect.Undefined)
                {
                    sbMark.Append($";{Aspect}");
                }

                if (Tense != GrammaticalTenses.Undefined)
                {
                    sbMark.Append($";{Tense}");
                }

                if (Voice != GrammaticalVoice.Undefined)
                {
                    sbMark.Append($";{Voice}");
                }

                if (AbilityModality != AbilityModality.Undefined && AbilityModality != AbilityModality.None)
                {
                    sbMark.Append($";{AbilityModality}");
                }

                if (PermissionModality != PermissionModality.Undefined && PermissionModality != PermissionModality.None)
                {
                    sbMark.Append($";{PermissionModality}");
                }

                if (ObligationModality != ObligationModality.Undefined && ObligationModality != ObligationModality.None)
                {
                    sbMark.Append($";{ObligationModality}");
                }

                if (ProbabilityModality != ProbabilityModality.Undefined && ProbabilityModality != ProbabilityModality.None)
                {
                    sbMark.Append($";{ProbabilityModality}");
                }

                if (ConditionalModality != ConditionalModality.Undefined && ConditionalModality != ConditionalModality.None)
                {
                    sbMark.Append($";{ConditionalModality}");
                }

                sbMark.Append(")");
            }

            sb.AppendLine($"{spaces}S{sbMark}");

            if (VocativePhrase != null)
            {
                sb.AppendLine($"{nextNspaces}Vocative:");
                sb.Append(VocativePhrase.ToDbgString(nextNextN));
            }

            if (Condition != null)
            {
                throw new NotImplementedException();
            }

            if (Subject != null)
            {
                sb.AppendLine($"{nextNspaces}Subject:");
                sb.Append(Subject.ToDbgString(nextNextN));
            }

            if (Predicate != null)
            {
                sb.AppendLine($"{nextNspaces}Predicate:");
                sb.Append(Predicate.ToDbgString(nextNextN));
            }

            return sb.ToString();
        }
    }
}
