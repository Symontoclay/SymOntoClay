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
        public override BaseGrammaticalWordFrame RootWordFrame => throw new NotImplementedException("035C7270-79B1-4F48-9F02-153CBE2CC33C");

        /// <inheritdoc/>
        public override string RootWordAsString => throw new NotImplementedException("8E0E4384-05F0-420B-885E-60D5910B94F3");

        /// <inheritdoc/>
        public override IList<string> GetConditionalLogicalMeaning(string conditionalWord)
        {
            throw new NotImplementedException("29E6E2BF-7EC2-4646-8807-E4CFCEBA4821");
        }

        /// <inheritdoc/>
        public override BaseSentenceItem CloneBaseSentenceItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Sentence Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public Sentence Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Sentence)context[this];
            }

            var result = new Sentence();
            context[this] = result;

            result.VocativePhrase = VocativePhrase?.CloneBaseSentenceItem(context);
            result.Condition = Condition?.CloneBaseSentenceItem(context);
            result.Subject = Subject?.CloneBaseSentenceItem(context);
            result.Predicate = Predicate?.CloneBaseSentenceItem(context);

            result.Mood = Mood;

            result.KindOfQuestion = KindOfQuestion;
            result.IsNegation = IsNegation;

            result.Aspect = Aspect;
            result.Tense = Tense;
            result.Voice = Voice;
            result.AbilityModality = AbilityModality;
            result.PermissionModality = PermissionModality;
            result.ObligationModality = ObligationModality;
            result.ProbabilityModality = ProbabilityModality;
            result.ConditionalModality = ConditionalModality;

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(VocativePhrase), VocativePhrase);
            sb.PrintObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(Subject), Subject);
            sb.PrintObjProp(n, nameof(Predicate), Predicate);


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
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNspaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            var sbMark = new StringBuilder();

            if((Mood != GrammaticalMood.Undefined && Mood != GrammaticalMood.Indicative) || (KindOfQuestion != KindOfQuestion.Undefined && KindOfQuestion != KindOfQuestion.None) || IsNegation ||
                (Aspect != GrammaticalAspect.Undefined && Aspect != GrammaticalAspect.Simple) || (Tense != GrammaticalTenses.Undefined && Tense != GrammaticalTenses.Present) || 
                (Voice != GrammaticalVoice.Undefined && Voice != GrammaticalVoice.Active) || (AbilityModality != AbilityModality.Undefined && AbilityModality != AbilityModality.None) ||
                (PermissionModality != PermissionModality.Undefined && PermissionModality != PermissionModality.None) || (ObligationModality != ObligationModality.Undefined && ObligationModality != ObligationModality.None) ||
                (ProbabilityModality != ProbabilityModality.Undefined && ProbabilityModality != ProbabilityModality.None) || (ConditionalModality != ConditionalModality.Undefined && ConditionalModality != ConditionalModality.None))
            {
                sbMark.Append(":(");

                var hasMarkItem = false;

                if (Mood != GrammaticalMood.Undefined && Mood != GrammaticalMood.Indicative)
                {
                    hasMarkItem = true;

                    sbMark.Append(Mood);
                }

                if (KindOfQuestion != KindOfQuestion.Undefined && KindOfQuestion != KindOfQuestion.None)
                {
                    if(hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{KindOfQuestion}");
                }

                if (IsNegation)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{IsNegation}");
                }

                if (Aspect != GrammaticalAspect.Undefined && Aspect != GrammaticalAspect.Simple)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{Aspect}");
                }

                if (Tense != GrammaticalTenses.Undefined && Tense != GrammaticalTenses.Present)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{Tense}");
                }

                if (Voice != GrammaticalVoice.Undefined && Voice != GrammaticalVoice.Active)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{Voice}");
                }

                if (AbilityModality != AbilityModality.Undefined && AbilityModality != AbilityModality.None)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{AbilityModality}");
                }

                if (PermissionModality != PermissionModality.Undefined && PermissionModality != PermissionModality.None)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{PermissionModality}");
                }

                if (ObligationModality != ObligationModality.Undefined && ObligationModality != ObligationModality.None)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{ObligationModality}");
                }

                if (ProbabilityModality != ProbabilityModality.Undefined && ProbabilityModality != ProbabilityModality.None)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }
                    else
                    {
                        hasMarkItem = true;
                    }

                    sbMark.Append($"{ProbabilityModality}");
                }

                if (ConditionalModality != ConditionalModality.Undefined && ConditionalModality != ConditionalModality.None)
                {
                    if (hasMarkItem)
                    {
                        sbMark.Append(";");
                    }

                    sbMark.Append($"{ConditionalModality}");
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
                throw new NotImplementedException("29BD60DE-F0A8-459B-9211-F83765249128");
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
