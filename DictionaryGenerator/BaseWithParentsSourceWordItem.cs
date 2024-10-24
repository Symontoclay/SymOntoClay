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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public abstract class BaseWithParentsSourceWordItem : IObjectToString, IRootWordBasedMetrixOfSourceWordNet
    {
        public int WordNum { get; set; }
        public string Word { get; set; }
        public IList<int> ParentWordNumsList { get; set; } = new List<int>();
        public bool IsDigit { get; set; }
        public bool IsName { get; set; }
        public bool IsComplex { get; set; }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(WordNum)} = {WordNum}");
            sb.AppendLine($"{spaces}{nameof(Word)} = {Word}");
            if (ParentWordNumsList == null)
            {
                sb.AppendLine($"{spaces}{nameof(ParentWordNumsList)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(ParentWordNumsList)}");
                foreach (var parentWordNum in ParentWordNumsList)
                {
                    sb.AppendLine($"{nextNSpaces}{parentWordNum}");
                }
                sb.AppendLine($"{spaces}End {nameof(ParentWordNumsList)}");
            }
            sb.AppendLine($"{spaces}{nameof(IsDigit)} = {IsDigit}");
            sb.AppendLine($"{spaces}{nameof(IsName)} = {IsName}");
            sb.AppendLine($"{spaces}{nameof(IsComplex)} = {IsComplex}");
            return sb.ToString();
        }
    }
}
