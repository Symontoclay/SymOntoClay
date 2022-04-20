using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class PrepositionGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Preposition;
        public override bool IsPreposition => true;
        public override PrepositionGrammaticalWordFrame AsPreposition => this;
        public GrammaticalComparison Comparison { get; set; } = GrammaticalComparison.None;

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new PrepositionGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Comparison = Comparison;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Comparison)} = {Comparison}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Comparison.GetHashCode();

            return result;
        }

        public static bool NEquals(PrepositionGrammaticalWordFrame left, PrepositionGrammaticalWordFrame right)
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

    public class ComparerOfPrepositionGrammaticalWordFrame : IEqualityComparer<PrepositionGrammaticalWordFrame>
    {
        bool IEqualityComparer<PrepositionGrammaticalWordFrame>.Equals(PrepositionGrammaticalWordFrame left, PrepositionGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return PrepositionGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<PrepositionGrammaticalWordFrame>.GetHashCode(PrepositionGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
