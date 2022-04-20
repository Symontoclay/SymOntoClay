using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class ConjunctionGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Conjunction;
        public override bool IsConjunction => true;
        public override ConjunctionGrammaticalWordFrame AsConjunction => this;
        public KindOfConjunction Kind { get; set; } = KindOfConjunction.Unknown;
        public SecondKindOfConjunction SecondKind { get; set; } = SecondKindOfConjunction.Unknown;
        public bool IsQuestionWord { get; set; }
        public bool IsNegation { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new ConjunctionGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Kind = Kind;
            result.SecondKind = SecondKind;
            result.IsQuestionWord = IsQuestionWord;
            result.IsNegation = IsNegation;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(SecondKind)} = {SecondKind}");
            sb.AppendLine($"{spaces}{nameof(IsQuestionWord)} = {IsQuestionWord}");
            sb.AppendLine($"{spaces}{nameof(IsNegation)} = {IsNegation}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Kind.GetHashCode() ^ SecondKind.GetHashCode() ^ IsQuestionWord.GetHashCode() ^ IsNegation.GetHashCode();
            return result;
        }

        public static bool NEquals(ConjunctionGrammaticalWordFrame left, ConjunctionGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return left.GetHashCode() == right.GetHashCode();
        }
    }

    public class ComparerOfConjunctionGrammaticalWordFrame : IEqualityComparer<ConjunctionGrammaticalWordFrame>
    {
        bool IEqualityComparer<ConjunctionGrammaticalWordFrame>.Equals(ConjunctionGrammaticalWordFrame left, ConjunctionGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return ConjunctionGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<ConjunctionGrammaticalWordFrame>.GetHashCode(ConjunctionGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
