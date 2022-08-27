/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public abstract class BaseGrammaticalWordFrame : IObjectToString
    {
        public abstract GrammaticalPartOfSpeech PartOfSpeech { get; }
        public virtual bool IsNoun => false;
        public virtual NounGrammaticalWordFrame AsNoun => null;
        public virtual bool IsPronoun => false;
        public virtual PronounGrammaticalWordFrame AsPronoun => null;
        public virtual bool IsAdjective => false;
        public virtual AdjectiveGrammaticalWordFrame AsAdjective => null;
        public virtual bool IsVerb => false;
        public virtual VerbGrammaticalWordFrame AsVerb => null;
        public virtual bool IsAdverb => false;
        public virtual AdverbGrammaticalWordFrame AsAdverb => null;
        public virtual bool IsPreposition => false;
        public virtual PrepositionGrammaticalWordFrame AsPreposition => null;
        public virtual bool IsPostposition => false;
        public virtual PostpositionGrammaticalWordFrame AsPostposition => null;
        public virtual bool IsConjunction => false;
        public virtual ConjunctionGrammaticalWordFrame AsConjunction => null;
        public virtual bool IsInterjection => false;
        public virtual InterjectionGrammaticalWordFrame AsInterjection => null;
        public virtual bool IsArticle => false;
        public virtual ArticleGrammaticalWordFrame AsArticle => null;
        public virtual bool IsNumeral => false;
        public virtual NumeralGrammaticalWordFrame AsNumeral => null;

        public string Word { get; set; }
        public string RootWord { get; set; }
        public IList<string> LogicalMeaning { get; set; }
        public IList<string> FullLogicalMeaning { get; set; }
        public IDictionary<string, IList<string>> ConditionalLogicalMeaning { get; set; }
        public bool IsArchaic { get; set; }
        public bool IsDialectal { get; set; }
        public bool IsPoetic { get; set; }
        public bool IsAbbreviation { get; set; }
        public bool IsRare { get; set; }

        protected void FillAsBaseGrammaticalWordFrame(BaseGrammaticalWordFrame item)
        {
            item.Word = Word;
            item.RootWord = RootWord;
            item.LogicalMeaning = LogicalMeaning?.ToList();
            item.FullLogicalMeaning = FullLogicalMeaning?.ToList();
            item.ConditionalLogicalMeaning = ConditionalLogicalMeaning?.ToDictionary(p => p.Key, p => p.Value?.ToList() as IList<string>);
            item.IsArchaic = IsArchaic;
            item.IsDialectal = IsDialectal;
            item.IsPoetic = IsPoetic;
            item.IsAbbreviation = IsAbbreviation;
            item.IsRare = IsRare;
        }

        public abstract BaseGrammaticalWordFrame Fork();

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + DisplayHelper.IndentationStep;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + DisplayHelper.IndentationStep;
            var nextNextNextNSpaces = DisplayHelper.Spaces(nextNextNextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(PartOfSpeech)} = {PartOfSpeech}");
            sb.AppendLine($"{spaces}{nameof(Word)} = {Word}");
            sb.AppendLine($"{spaces}{nameof(RootWord)} = {RootWord}");

            if (LogicalMeaning == null)
            {
                sb.AppendLine($"{spaces}{nameof(LogicalMeaning)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(LogicalMeaning)}");
                foreach (var item in LogicalMeaning)
                {
                    sb.AppendLine($"{nextNSpaces}{item}");
                }
                sb.AppendLine($"{spaces}End {nameof(LogicalMeaning)}");
            }

            if (FullLogicalMeaning == null)
            {
                sb.AppendLine($"{spaces}{nameof(FullLogicalMeaning)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(FullLogicalMeaning)}");
                foreach (var item in FullLogicalMeaning)
                {
                    sb.AppendLine($"{nextNSpaces}{item}");
                }
                sb.AppendLine($"{spaces}End {nameof(FullLogicalMeaning)}");
            }

            if (ConditionalLogicalMeaning == null)
            {
                sb.AppendLine($"{spaces}{nameof(ConditionalLogicalMeaning)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ConditionalLogicalMeaning)}");

                foreach (var kvpItems in ConditionalLogicalMeaning)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {kvpItems.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Meanings");
                    var meaningsList = kvpItems.Value;
                    foreach (var meaning in meaningsList)
                    {
                        sb.AppendLine($"{nextNextNextNSpaces}{meaning}");
                    }
                    sb.AppendLine($"{nextNextNSpaces}End Meanings");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(ConditionalLogicalMeaning)}");
            }

            sb.AppendLine($"{spaces}{nameof(IsArchaic)} = {IsArchaic}");
            sb.AppendLine($"{spaces}{nameof(IsDialectal)} = {IsDialectal}");
            sb.AppendLine($"{spaces}{nameof(IsPoetic)} = {IsPoetic}");
            sb.AppendLine($"{spaces}{nameof(IsAbbreviation)} = {IsAbbreviation}");
            sb.AppendLine($"{spaces}{nameof(IsRare)} = {IsRare}");

            return sb.ToString();
        }

        protected int GetHashCodeOfBaseFrame()
        {
            var result = PartOfSpeech.GetHashCode();

            if (!string.IsNullOrWhiteSpace(RootWord))
            {
                result ^= RootWord.GetHashCode();
            }

            result ^= IsArchaic.GetHashCode() ^ IsDialectal.GetHashCode() ^ IsPoetic.GetHashCode() ^ IsAbbreviation.GetHashCode() ^ IsRare.GetHashCode();
            return result;
        }

        public static bool NBaseEquals(BaseGrammaticalWordFrame left, BaseGrammaticalWordFrame right)
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

    public class ComparerOfBaseGrammaticalWordFrame : IEqualityComparer<BaseGrammaticalWordFrame>
    {
        bool IEqualityComparer<BaseGrammaticalWordFrame>.Equals(BaseGrammaticalWordFrame left, BaseGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return BaseGrammaticalWordFrame.NBaseEquals(left, right);
        }

        int IEqualityComparer<BaseGrammaticalWordFrame>.GetHashCode(BaseGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
