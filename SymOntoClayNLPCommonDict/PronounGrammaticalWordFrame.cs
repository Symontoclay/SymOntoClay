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
    public class PronounGrammaticalWordFrame : BaseGrammaticalWordFrame
    {
        public override GrammaticalPartOfSpeech PartOfSpeech => GrammaticalPartOfSpeech.Pronoun;
        public override bool IsPronoun => true;
        public override PronounGrammaticalWordFrame AsPronoun => this;
        public GrammaticalGender Gender { get; set; } = GrammaticalGender.Neuter;
        public GrammaticalNumberOfWord Number { get; set; } = GrammaticalNumberOfWord.Neuter;
        public GrammaticalPerson Person { get; set; } = GrammaticalPerson.Neuter;
        public TypeOfPronoun TypeOfPronoun { get; set; } = TypeOfPronoun.Undefined;
        public CaseOfPersonalPronoun Case { get; set; } = CaseOfPersonalPronoun.Undefined;
        public bool IsQuestionWord { get; set; }
        public bool IsNegation { get; set; }

        public override BaseGrammaticalWordFrame Fork()
        {
            var result = new PronounGrammaticalWordFrame();
            FillAsBaseGrammaticalWordFrame(result);
            result.Gender = Gender;
            result.Number = Number;
            result.Person = Person;
            result.TypeOfPronoun = TypeOfPronoun;
            result.Case = Case;
            result.IsQuestionWord = IsQuestionWord;
            result.IsNegation = IsNegation;
            return result;
        }

        public override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));
            sb.AppendLine($"{spaces}{nameof(Gender)} = {Gender}");
            sb.AppendLine($"{spaces}{nameof(Number)} = {Number}");
            sb.AppendLine($"{spaces}{nameof(Person)} = {Person}");
            sb.AppendLine($"{spaces}{nameof(TypeOfPronoun)} = {TypeOfPronoun}");
            sb.AppendLine($"{spaces}{nameof(Case)} = {Case}");
            sb.AppendLine($"{spaces}{nameof(IsQuestionWord)} = {IsQuestionWord}");
            sb.AppendLine($"{spaces}{nameof(IsNegation)} = {IsNegation}");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var result = GetHashCodeOfBaseFrame();
            result ^= Gender.GetHashCode() ^ Number.GetHashCode() ^ Person.GetHashCode() ^ TypeOfPronoun.GetHashCode() ^ Case.GetHashCode() ^ IsQuestionWord.GetHashCode() ^ IsNegation.GetHashCode();
            return result;
        }

        public static bool NEquals(PronounGrammaticalWordFrame left, PronounGrammaticalWordFrame right)
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

    public class ComparerOfPronounGrammaticalWordFrame : IEqualityComparer<PronounGrammaticalWordFrame>
    {
        bool IEqualityComparer<PronounGrammaticalWordFrame>.Equals(PronounGrammaticalWordFrame left, PronounGrammaticalWordFrame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return PronounGrammaticalWordFrame.NEquals(left, right);
        }

        int IEqualityComparer<PronounGrammaticalWordFrame>.GetHashCode(PronounGrammaticalWordFrame obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
