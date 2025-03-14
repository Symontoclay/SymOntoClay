using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class TypeInfo : Value, IEquatable<TypeInfo>
    {
        public static TypeInfo Empty = new TypeInfo();
        public static TypeInfo NullTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.NullTypeName));
        public static TypeInfo NumberTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.NumberTypeName));
        public static TypeInfo FuzzyTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName));
        public static TypeInfo StringTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.StringTypeName));
        public static TypeInfo SequenceTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.SequenceTypeName));
        public static TypeInfo EntityTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.EntityTypeName));
        public static TypeInfo StrongIdentifierTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.StrongIdentifierTypeName));
        public static TypeInfo WaypointTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.WaypointTypeName));
        public static TypeInfo ConditionalEntityTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.ConditionalEntityTypeName));
        public static TypeInfo FactTypeTypeInfo = new TypeInfo(NameHelper.CreateName(StandardNamesConstants.FactTypeName));

        public TypeInfo()
        {
        }

        public TypeInfo(StrongIdentifierValue name)
            : this(false, name, false, null)
        {
        }

        public TypeInfo(StrongIdentifierValue name, bool іsArray, int? capacity)
            : this(false, name, іsArray, capacity)
        {
        }

        public TypeInfo(bool isAnonymous, StrongIdentifierValue name, bool іsArray, int? capacity)
        {
            IsAnonymous = isAnonymous;
            Name = Name;
            IsArray = IsArray;
            Capacity = capacity;
            _isEmpty = false;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.TypeInfo;

        /// <inheritdoc/>
        public override bool IsTypeInfo => true;

        /// <inheritdoc/>
        public override TypeInfo AsTypeInfo => this;

        public bool IsAnonymous { get; set; }
        public StrongIdentifierValue Name { get; private set; }
        public bool IsArray { get; private set; }
        public int? Capacity { get; private set; }

        private readonly bool _isEmpty = true;

        public bool IsEmpty => _isEmpty || (Name?.IsEmpty ?? true);

        public KindOfName KindOfName => Name?.KindOfName ?? KindOfName.Unknown;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return null;
        }

        /// <inheritdoc/>
        public bool Equals(TypeInfo other)
        {
            return NEquals(other);
        }

        private bool NEquals(TypeInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if(IsAnonymous != other.IsAnonymous)
            {
                return false;
            }

            if(!Name.Equals(other.Name))
            {
                return false;
            }

            if(IsArray != other.IsArray)
            {
                return false;
            }

            if(Capacity != other.Capacity)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as TypeInfo;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        public static bool operator == (TypeInfo a, TypeInfo b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null))
            {
                return false;
            }

            if (ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator != (TypeInfo a, TypeInfo b)
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

            return !a.Equals(b); 
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            Name?.CheckDirty();

            return base.CalculateLongHashCode(options) ^ (Name?.GetLongHashCode() ?? 0) ^ (Capacity.HasValue ? (ulong)Capacity.Value : 0);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public TypeInfo Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public TypeInfo Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (TypeInfo)context[this];
            }

            var result = new TypeInfo();
            context[this] = result;

            result.IsAnonymous = IsAnonymous;
            result.Name = Name?.Clone(context);
            result.IsArray = IsArray;
            result.Capacity = Capacity;
            //result._values = _values?.Select(p => p.CloneValue(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            //sb.PrintObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            //sb.PrintShortObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(IsArray)} = {IsArray}");
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            //sb.PrintBriefObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if(!IsAnonymous)
            {
                sb.Append(Name.ToHumanizedString());
            }

            if(IsArray)
            {
                sb.Append("[");

                if(Capacity.HasValue)
                {
                    sb.Append(Capacity);
                }

                sb.Append("]");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }
    }
}
