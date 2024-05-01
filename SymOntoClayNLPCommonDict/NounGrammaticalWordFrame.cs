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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.CommonDict
{
    public class NounGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Noun;
        public override bool IsNoun => true;
        public override NounGrammaticalWordFrame AsNoun => this;
        public bool IsName { get; set; }
        public bool IsShortForm { get; set; }
        public GrammaticalGender Gender { get; set; } = GrammaticalGender.Neuter;
        public GrammaticalNumberOfWord Number { get; set; } = GrammaticalNumberOfWord.Neuter;
        public bool IsCountable { get; set; }
        public bool IsGerund { get; set; }
        /// <summary>
        /// Example: `father's` is possessive, `father` is not possessive. 
        /// </summary>
        public bool IsPossessive { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new NounGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.IsName = IsName;
            result.IsShortForm = IsShortForm;
            result.Gender = Gender;
            result.Number = Number;
            result.IsCountable = IsCountable;
            result.IsGerund = IsGerund;
            result.IsPossessive = IsPossessive;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(IsName)} = {IsName}");
            sb.AppendLine($"{spaces}{nameof(IsShortForm)} = {IsShortForm}");
            sb.AppendLine($"{spaces}{nameof(Gender)} = {Gender}");
            sb.AppendLine($"{spaces}{nameof(Number)} = {Number}");
            sb.AppendLine($"{spaces}{nameof(IsCountable)} = {IsCountable}");
            sb.AppendLine($"{spaces}{nameof(IsGerund)} = {IsGerund}");
            sb.AppendLine($"{spaces}{nameof(IsPossessive)} = {IsPossessive}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= IsName.GetHashCode() ^ IsShortForm.GetHashCode() ^ Gender.GetHashCode() ^ Number.GetHashCode() ^ IsCountable.GetHashCode() ^ IsGerund.GetHashCode() ^ IsPossessive.GetHashCode();
            return result;
        }

        public static bool NEquals(NounGrammaticalWordFrame left, NounGrammaticalWordFrame right)
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

    public class ComparerOfNounGrammaticalWordFrame : IEqualityComparer<NounGrammaticalWordFrame>
    {
        bool IEqualityComparer<NounGrammaticalWordFrame>.Equals(NounGrammaticalWordFrame left, NounGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return NounGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<NounGrammaticalWordFrame>.GetHashCode(NounGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
