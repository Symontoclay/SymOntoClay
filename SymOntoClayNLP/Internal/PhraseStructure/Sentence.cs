using SymOntoClay.CoreHelper.DebugHelpers;
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

        public BaseSentenceItem Condition { get; set; }
        public BaseSentenceItem Subject { get; set; }
        public BaseSentenceItem Predicate { get; set; }

        public GrammaticalMood Mood { get; set; } = GrammaticalMood.Undefined;

        public bool IsQuestion { get; set; }
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
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(Subject), Subject);
            sb.PrintObjProp(n, nameof(Predicate), Predicate);

            //sb.PrintObjProp(n, nameof(), );

            sb.AppendLine($"{spaces}{nameof(Mood)} = {Mood}");

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");
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

            if(Mood != GrammaticalMood.Undefined)
            {
                throw new NotImplementedException();
            }

            if(IsQuestion)
            {
                throw new NotImplementedException();
            }

            if (IsNegation)
            {
                throw new NotImplementedException();
            }

            if (Aspect != GrammaticalAspect.Undefined)
            {
                throw new NotImplementedException();
            }

            if (Tense != GrammaticalTenses.Undefined)
            {
                throw new NotImplementedException();
            }

            if (Voice != GrammaticalVoice.Undefined)
            {
                throw new NotImplementedException();
            }

            if (AbilityModality != AbilityModality.Undefined)
            {
                throw new NotImplementedException();
            }

            if (PermissionModality != PermissionModality.Undefined)
            {
                throw new NotImplementedException();
            }

            if (ObligationModality != ObligationModality.Undefined)
            {
                throw new NotImplementedException();
            }

            if (ProbabilityModality != ProbabilityModality.Undefined)
            {
                throw new NotImplementedException();
            }

            if (ConditionalModality != ConditionalModality.Undefined)
            {
                throw new NotImplementedException();
            }

            sb.AppendLine($"{spaces}S");

            if(Condition != null)
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
