/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StrongIdentifierValue: Value, IEquatable<StrongIdentifierValue>
    {
        public static readonly StrongIdentifierValue LogicalVarBlankIdentifier = NameHelper.CreateName("$_");
        public static readonly StrongIdentifierValue Empty = new StrongIdentifierValue();

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.StrongIdentifierValue;

        /// <inheritdoc/>
        public override bool IsStrongIdentifierValue => true;

        /// <inheritdoc/>
        public override StrongIdentifierValue AsStrongIdentifierValue => this;

        public bool IsEmpty { get; set; } = true;

        public KindOfName KindOfName { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; } = string.Empty;

        public string NormalizedNameValue { get; set; } = string.Empty;
        public string NameWithoutPrefix { get; set; } = string.Empty;

        private bool _isNull;

        public bool IsArray { get; set; }
        public int? Capacity { get; set; }
        public bool HasInfiniteCapacity { get; set; }

        public StrongIdentifierLevel Level { get; set; } = StrongIdentifierLevel.None;

        public List<StrongIdentifierValue> Namespaces { get; set; } = new List<StrongIdentifierValue>();

        public StrongIdentifierValue ForResolving { get; set; } = null;

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
            return NEquals(other);
        }

        private bool NEquals(StrongIdentifierValue other)
        {
            if (other == null)
            {
                return false;
            }

            if (IsEmpty != other.IsEmpty)
            {
                return false;
            }

            if (IsArray != other.IsArray)
            {
                return false;
            }

            if (Capacity != other.Capacity)
            {
                return false;
            }

            if(HasInfiniteCapacity != other.HasInfiniteCapacity)
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
            
            return NEquals(personObj);
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
            return a.NEquals(b);
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
            
            return !a.NEquals(b);
        }

        /// <inheritdoc/>
        public override bool NullValueEquals()
        {
            return _isNull;
        }

        /// <inheritdoc/>
        protected override bool ConcreteValueEquals(Value other)
        {
            return NEquals(other.AsStrongIdentifierValue);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return NormalizedNameValue.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            if(NormalizedNameValue == "null")
            {
                _isNull = true;
                return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.NullWeight;
            }

            return base.CalculateLongHashCode(options) ^ (ulong)NormalizedNameValue.GetHashCode();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public StrongIdentifierValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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
            result.NameWithoutPrefix = NameWithoutPrefix;
            result.IsArray = IsArray;
            result.Capacity = Capacity;
            result.HasInfiniteCapacity = HasInfiniteCapacity;
            result.Level = Level;
            result.Namespaces = Namespaces?.Select(p => p.Clone(context))?.ToList();
            result.ForResolving = ForResolving?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
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
            sb.AppendLine($"{spaces}{nameof(NameWithoutPrefix)} = {NameWithoutPrefix}");

            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            sb.AppendLine($"{spaces}{nameof(HasInfiniteCapacity)} = {HasInfiniteCapacity}");

            sb.AppendLine($"{spaces}{nameof(Level)} = {Level}");

            sb.PrintObjListProp(n, nameof(Namespaces), Namespaces);

            sb.PrintBriefObjProp(n, nameof(ForResolving), ForResolving);

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
            sb.AppendLine($"{spaces}{nameof(NameWithoutPrefix)} = {NameWithoutPrefix}");

            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            sb.AppendLine($"{spaces}{nameof(HasInfiniteCapacity)} = {HasInfiniteCapacity}");

            sb.AppendLine($"{spaces}{nameof(Level)} = {Level}");

            sb.PrintShortObjListProp(n, nameof(Namespaces), Namespaces);

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
            sb.AppendLine($"{spaces}{nameof(NameWithoutPrefix)} = {NameWithoutPrefix}");

            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            sb.AppendLine($"{spaces}{nameof(HasInfiniteCapacity)} = {HasInfiniteCapacity}");

            sb.AppendLine($"{spaces}{nameof(Level)} = {Level}");

            sb.PrintBriefObjListProp(n, nameof(Namespaces), Namespaces);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            return $"{spaces}{NToHumanizedString(DebugHelperOptions.FromHumanizedOptions())}";
        }

        /// <inheritdoc/>
        public override object ToMonitorSerializableObject(IMonitorLogger logger)
        {
            return NToHumanizedString(DebugHelperOptions.FromHumanizedOptions());
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString(options);
        }

        private string NToHumanizedString(DebugHelperOptions options)
        {
            if(IsEmpty)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            switch(Level)
            {
                case StrongIdentifierLevel.None:
                    break;

                case StrongIdentifierLevel.Global:
                    sb.Append("global::");
                    break;

                case StrongIdentifierLevel.Root:
                    sb.Append("root::");
                    break;

                case StrongIdentifierLevel.Strategic:
                    sb.Append("strategic::");
                    break;

                case StrongIdentifierLevel.Tactical:
                    sb.Append("tactical::");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Level), Level, null);
            }

            switch(KindOfName)
            {
                case KindOfName.CommonConcept:
                case KindOfName.Entity:
                case KindOfName.RuleOrFact:
                case KindOfName.EntityCondition:
                case KindOfName.OnceResolvedEntityCondition:
                case KindOfName.AnonymousEntityCondition:
                case KindOfName.OnceResolvedAnonymousEntityCondition:
                case KindOfName.Channel:
                case KindOfName.Var:
                case KindOfName.SystemVar:
                case KindOfName.LogicalVar:                
                case KindOfName.Property:
                        sb.Append(NameValue);
                    break;

                case KindOfName.Concept:
                case KindOfName.LinguisticVar:
                    if(options.ShowPrefixesForConceptLikeIdentifier)
                    {
                        sb.Append(NameValue);
                    }
                    else
                    {
                        sb.Append(NameWithoutPrefix);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfName), KindOfName, null);
            }

            if (IsArray)
            {
                sb.Append("[");

                if (Capacity.HasValue)
                {
                    sb.Append(Capacity);
                } 
                else
                {
                    sb.Append('∞');
                }

                sb.Append("]");
            }

            if (Namespaces.Any())
            {
                sb.Append($" ({string.Join(" | ", Namespaces.Select(p => p.ToHumanizedLabel()))})");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString(DebugHelperOptions.FromHumanizedOptions())
            };
        }

        private static bool IsSystemNullOrEmpty(StrongIdentifierValue value)
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
