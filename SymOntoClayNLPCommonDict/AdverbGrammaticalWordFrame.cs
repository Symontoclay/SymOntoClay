using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class AdverbGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Adverb;
        public override bool IsAdverb => true;
        public override AdverbGrammaticalWordFrame AsAdverb => this;
        public GrammaticalComparison Comparison { get; set; } = GrammaticalComparison.None;
        public bool IsQuestionWord { get; set; }
        public bool IsDeterminer { get; set; }
        public bool IsNegation { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new AdverbGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Comparison = Comparison;
            result.IsQuestionWord = IsQuestionWord;
            result.IsDeterminer = IsDeterminer;
            result.IsNegation = IsNegation;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Comparison)} = {Comparison}");
            sb.AppendLine($"{spaces}{nameof(IsQuestionWord)} = {IsQuestionWord}");
            sb.AppendLine($"{spaces}{nameof(IsDeterminer)} = {IsDeterminer}");
            sb.AppendLine($"{spaces}{nameof(IsNegation)} = {IsNegation}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Comparison.GetHashCode() ^ IsQuestionWord.GetHashCode() ^ IsDeterminer.GetHashCode() ^ IsNegation.GetHashCode();
            return result;
        }

        public static bool NEquals(AdverbGrammaticalWordFrame left, AdverbGrammaticalWordFrame right)
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

    public class ComparerOfAdverbGrammaticalWordFrame : IEqualityComparer<AdverbGrammaticalWordFrame>
    {
        bool IEqualityComparer<AdverbGrammaticalWordFrame>.Equals(AdverbGrammaticalWordFrame left, AdverbGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return AdverbGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<AdverbGrammaticalWordFrame>.GetHashCode(AdverbGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
