/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
    public class IndexedStrongIdentifierValue : IndexedValue, IEquatable<IndexedStrongIdentifierValue>
    {
        public StrongIdentifierValue OriginalStrongIdentifierValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalStrongIdentifierValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.StrongIdentifierValue;

        public string DictionaryName { get; set; }

        public bool IsEmpty { get; set; } = true;

        public KindOfName KindOfName { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; } = string.Empty;
        public ulong NameKey { get; set; }

        /// <inheritdoc/>
        public override bool IsStrongIdentifierValue => true;

        /// <inheritdoc/>
        public override IndexedStrongIdentifierValue AsStrongIdentifierValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return NameValue;
        }

        /// <inheritdoc/>
        public bool Equals(IndexedStrongIdentifierValue other)
        {
            if (other == null)
            {
                return false;
            }

            return NameKey == other.NameKey;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as StrongIdentifierValue;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return NameKey.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ NameKey;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}`{NameValue}`";
        }
    }
}
