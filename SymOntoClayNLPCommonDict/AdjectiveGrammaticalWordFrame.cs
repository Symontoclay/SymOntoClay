using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class AdjectiveGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Adjective;
        public override bool IsAdjective => true;
        public override AdjectiveGrammaticalWordFrame AsAdjective => this;
        public GrammaticalComparison Comparison { get; set; } = GrammaticalComparison.None;
        public bool IsDeterminer { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new AdjectiveGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Comparison = Comparison;
            result.IsDeterminer = IsDeterminer;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Comparison)} = {Comparison}");
            sb.AppendLine($"{spaces}{nameof(IsDeterminer)} = {IsDeterminer}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Comparison.GetHashCode() ^ IsDeterminer.GetHashCode();
            return result;
        }

        public static bool NEquals(AdjectiveGrammaticalWordFrame left, AdjectiveGrammaticalWordFrame right)
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

    public class ComparerOfAdjectiveGrammaticalWordFrame : IEqualityComparer<AdjectiveGrammaticalWordFrame>
    {
        bool IEqualityComparer<AdjectiveGrammaticalWordFrame>.Equals(AdjectiveGrammaticalWordFrame left, AdjectiveGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return AdjectiveGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<AdjectiveGrammaticalWordFrame>.GetHashCode(AdjectiveGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
