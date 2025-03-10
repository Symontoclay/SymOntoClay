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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using SymOntoClay.Common.DebugHelpers;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class WaypointValue : LoggedValue, INavTarget
    {
        public WaypointValue(float distance, IEngineContext context)
            : this(distance, 0f, new StrongIdentifierValue(), context)
        {
        }

        public WaypointValue(float distance, float horizontalAngle, IEngineContext context)
            : this(distance, horizontalAngle, new StrongIdentifierValue(), context)
        {
        }

        public WaypointValue(float distance, float horizontalAngle, StrongIdentifierValue name, IEngineContext context)
            : base(context.Logger)
        {
            Distance = distance;
            HorizontalAngle = horizontalAngle;

            _context = context;

            AbsoluteCoordinates = context.HostSupport.ConvertFromRelativeToAbsolute(Logger, new RelativeCoordinate() { Distance = distance, HorizontalAngle = horizontalAngle});
            Name = name;
        }

        private WaypointValue(float distance, float horizontalAngle, Vector3 absoluteCoordinates, StrongIdentifierValue name, IEngineContext context)
            : base(context.Logger)
        {
            Distance = distance;
            HorizontalAngle = horizontalAngle;
            Name = name;
            AbsoluteCoordinates = absoluteCoordinates;
            _context = context;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.WaypointValue;

        /// <inheritdoc/>
        public override bool IsWaypointValue => true;

        /// <inheritdoc/>
        public override WaypointValue AsWaypointValue => this;

        public float Distance { get; private set; }
        public float HorizontalAngle { get; private set; }
        public Vector3 AbsoluteCoordinates { get; private set; }
        public StrongIdentifierValue Name { get; private set; }

        private IEngineContext _context;

        private List<TypeInfo> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<TypeInfo> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return AbsoluteCoordinates;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("1E92DEF1-425E-4329-BCD9-84E3AB1EEF65");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.WaypointTypeName) };

            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode(options) ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ (ulong)Math.Abs(AbsoluteCoordinates.GetHashCode()) ^ (ulong)Math.Abs(Distance.GetHashCode()) ^ (ulong)Math.Abs(HorizontalAngle.GetHashCode());
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new WaypointValue(Distance, HorizontalAngle, AbsoluteCoordinates, Name?.Clone(context), _context);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        KindOfNavTarget INavTarget.Kind => KindOfNavTarget.ByAbsoluteCoordinates;

        /// <inheritdoc/>
        IEntity INavTarget.Entity => null;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(HorizontalAngle)} = {HorizontalAngle}");
            sb.AppendLine($"{spaces}{nameof(AbsoluteCoordinates)} = {AbsoluteCoordinates}");
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(HorizontalAngle)} = {HorizontalAngle}");
            sb.AppendLine($"{spaces}{nameof(AbsoluteCoordinates)} = {AbsoluteCoordinates}");
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(HorizontalAngle)} = {HorizontalAngle}");
            sb.AppendLine($"{spaces}{nameof(AbsoluteCoordinates)} = {AbsoluteCoordinates}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);

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
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            if (Name != null && !Name.IsEmpty)
            {
                return $"{Name.NameValue}[{Distance.ToString(CultureInfo.InvariantCulture)}, {HorizontalAngle.ToString(CultureInfo.InvariantCulture)}]";
            }

            return $"#@[{Distance.ToString(CultureInfo.InvariantCulture)}, {HorizontalAngle.ToString(CultureInfo.InvariantCulture)}]";
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
