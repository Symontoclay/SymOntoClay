using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class PostpositionGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Postposition;
        public override bool IsPostposition => true;
        public override PostpositionGrammaticalWordFrame AsPostposition => this;

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new PostpositionGrammaticalWordFrame();
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

        public static bool NEquals(PostpositionGrammaticalWordFrame left, PostpositionGrammaticalWordFrame right)
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

    public class ComparerOfPostpositionGrammaticalWordFrame : IEqualityComparer<PostpositionGrammaticalWordFrame>
    {
        bool IEqualityComparer<PostpositionGrammaticalWordFrame>.Equals(PostpositionGrammaticalWordFrame left, PostpositionGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return PostpositionGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<PostpositionGrammaticalWordFrame>.GetHashCode(PostpositionGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
