using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class ArticleGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Article;
        public override bool IsArticle => true;
        public override ArticleGrammaticalWordFrame AsArticle => this;
        public GrammaticalNumberOfWord Number { get; set; } = GrammaticalNumberOfWord.Neuter;
        public bool IsDeterminer => true;
        public KindOfArticle Kind { get; set; } = KindOfArticle.Unknown;

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new ArticleGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Number = Number;
            result.Kind = Kind;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Number)} = {Number}");
            sb.AppendLine($"{spaces}{nameof(IsDeterminer)} = {IsDeterminer}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Number.GetHashCode() ^ IsDeterminer.GetHashCode() ^ Kind.GetHashCode();
            return result;
        }

        public static bool NEquals(ArticleGrammaticalWordFrame left, ArticleGrammaticalWordFrame right)
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

    public class ComparerOfArticleGrammaticalWordFrame : IEqualityComparer<ArticleGrammaticalWordFrame>
    {
        bool IEqualityComparer<ArticleGrammaticalWordFrame>.Equals(ArticleGrammaticalWordFrame left, ArticleGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return ArticleGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<ArticleGrammaticalWordFrame>.GetHashCode(ArticleGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
