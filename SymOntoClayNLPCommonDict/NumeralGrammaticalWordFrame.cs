using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class NumeralGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Numeral;
        public override bool IsNumeral => true;
        public override NumeralGrammaticalWordFrame AsNumeral => this;
        public NumeralType NumeralType { get; set; } = NumeralType.Undefined;
        public float? RepresentedNumber { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new NumeralGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.NumeralType = NumeralType;
            result.RepresentedNumber = RepresentedNumber;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(NumeralType)} = {NumeralType}");
            sb.AppendLine($"{spaces}{nameof(RepresentedNumber)} = {RepresentedNumber}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= NumeralType.GetHashCode();

            if (RepresentedNumber.HasValue)
            {
                result ^= RepresentedNumber.Value.GetHashCode();
            }

            return result;
        }

        public static bool NEquals(NumeralGrammaticalWordFrame left, NumeralGrammaticalWordFrame right)
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

    public class ComparerOfNumeralGrammaticalWordFrame : IEqualityComparer<NumeralGrammaticalWordFrame>
    {
        bool IEqualityComparer<NumeralGrammaticalWordFrame>.Equals(NumeralGrammaticalWordFrame left, NumeralGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return NumeralGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<NumeralGrammaticalWordFrame>.GetHashCode(NumeralGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
