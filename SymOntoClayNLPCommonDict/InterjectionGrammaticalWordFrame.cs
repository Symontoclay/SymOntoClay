using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class InterjectionGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Interjection;
        public override bool IsInterjection => true;
        public override InterjectionGrammaticalWordFrame AsInterjection => this;

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new InterjectionGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            return result;
        }

        public static bool NEquals(InterjectionGrammaticalWordFrame left, InterjectionGrammaticalWordFrame right)
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

    public class ComparerOfInterjectionGrammaticalWordFrame : IEqualityComparer<InterjectionGrammaticalWordFrame>
    {
        bool IEqualityComparer<InterjectionGrammaticalWordFrame>.Equals(InterjectionGrammaticalWordFrame left, InterjectionGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return InterjectionGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<InterjectionGrammaticalWordFrame>.GetHashCode(InterjectionGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
