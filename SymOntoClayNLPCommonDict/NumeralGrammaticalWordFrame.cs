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
