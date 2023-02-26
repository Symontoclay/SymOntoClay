/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StrongIdentifierValue: Value, IEquatable<StrongIdentifierValue>
    {
        public static readonly StrongIdentifierValue LogicalVarBlankIdentifier = NameHelper.CreateName("$_");
        public static readonly StrongIdentifierValue Empty = new StrongIdentifierValue();

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.StrongIdentifierValue;

        public bool IsEmpty { get; set; } = true;

        public KindOfName KindOfName { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; } = string.Empty;

        public string NormalizedNameValue { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override bool IsStrongIdentifierValue => true;

        /// <inheritdoc/>
        public override StrongIdentifierValue AsStrongIdentifierValue => this;

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes
        {
            get
            {
                if(_builtInSuperTypes == null)
                {
                    _builtInSuperTypes = new List<StrongIdentifierValue>() { this };
                }

                return _builtInSuperTypes;
            }
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return NormalizedNameValue;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return NameValue;
        }

        /// <inheritdoc/>
        public bool Equals(StrongIdentifierValue other)
        {
            if (other == null)
            {
                return false;
            }

            return NormalizedNameValue == other.NormalizedNameValue;
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

        public static bool operator == (StrongIdentifierValue a, StrongIdentifierValue b)
        {
            if(ReferenceEquals(a, b))
            {
                return true;
            }

            if(ReferenceEquals(a, null))
            {
                return false;
            }

            if (ReferenceEquals(b, null))
            {
                return false;
            }

            return a.NormalizedNameValue == b.NormalizedNameValue;
        }

        public static bool operator != (StrongIdentifierValue a, StrongIdentifierValue b)
        {
            if (ReferenceEquals(a, b))
            {
                return false;
            }

            if (ReferenceEquals(a, null))
            {
                return true;
            }

            if (ReferenceEquals(b, null))
            {
                return true;
            }

            return a.NormalizedNameValue != b.NormalizedNameValue;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return NormalizedNameValue.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)NormalizedNameValue.GetHashCode();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public StrongIdentifierValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public StrongIdentifierValue Clone(Dictionary<object, object> context)
        {
            if(context.ContainsKey(this))
            {
                return (StrongIdentifierValue)context[this];
            }

            var result = new StrongIdentifierValue();
            context[this] = result;

            result.IsEmpty = IsEmpty;

            result.KindOfName = KindOfName;
            result.NameValue = NameValue;
            result.NormalizedNameValue = NormalizedNameValue;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NormalizedNameValue)} = {NormalizedNameValue}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NormalizedNameValue)} = {NormalizedNameValue}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(KindOfName)} = {KindOfName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NormalizedNameValue)} = {NormalizedNameValue}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}`{NameValue}`";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NameValue;
        }

        private static bool IsNullOrEmpty(StrongIdentifierValue value)
        {
            if(value == null)
            {
                return true;
            }

            if(value.IsEmpty)
            {
                return true;
            }

            return false;
        }
    }
}
