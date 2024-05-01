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

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RangeBoundary: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public NumberValue Value { get; set; }
        public bool Includes { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public RangeBoundary Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public RangeBoundary Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RangeBoundary)context[this];
            }

            var result = new RangeBoundary();
            context[this] = result;

            result.Value = Value?.Clone(context);
            result.Includes = Includes;

            return result;
        }

        private bool _isDirty = true;

        public void CheckDirty(CheckDirtyOptions options)
        {
            if (_isDirty)
            {
                CalculateLongHashCodes(options);
                _isDirty = false;
            }
        }

        private ulong? _longHashCode;

        public ulong GetLongHashCode()
        {
            return GetLongHashCode(null);
        }

        public ulong GetLongHashCode(CheckDirtyOptions options)
        {
            if (!_longHashCode.HasValue)
            {
                CalculateLongHashCodes(options);
            }

            return _longHashCode.Value;
        }

        public void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            Value.CheckDirty(options);

            _longHashCode = Value.GetLongHashCode() ^ (ulong)Includes.GetHashCode();

            _isDirty = false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }
    }
}
