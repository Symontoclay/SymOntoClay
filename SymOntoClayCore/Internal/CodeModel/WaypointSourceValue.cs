/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class WaypointSourceValue : Value
    {
        public WaypointSourceValue(Value distance, Value horizontalAngle, StrongIdentifierValue name)
        {
            Distance = distance;

            if(horizontalAngle == null)
            {
                HorizontalAngle = new NumberValue(0);
            }
            else
            {
                HorizontalAngle = horizontalAngle;
            }

            if(name == null)
            {
                Name = new StrongIdentifierValue();
            }
            else
            {
                Name = name;
            }
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.WaypointSourceValue;

        /// <inheritdoc/>
        public override bool IsWaypointSourceValue => true;

        /// <inheritdoc/>
        public override WaypointSourceValue AsWaypointSourceValue => this;

        public Value Distance { get; private set; }
        public Value HorizontalAngle { get; private set; }
        public StrongIdentifierValue Name { get; private set; }

        public WaypointValue ConvertToWaypointValue(IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var numberValueLinearResolver = context.DataResolversFactory.GetNumberValueLinearResolver();

            var resolvedFirstParam = numberValueLinearResolver.Resolve(Distance, localContext, ResolverOptions.GetDefaultOptions());

            var resolvedSecondParam = numberValueLinearResolver.Resolve(HorizontalAngle, localContext, ResolverOptions.GetDefaultOptions());

            var result = new WaypointValue((float)(double)resolvedFirstParam.GetSystemValue(), (float)(double)resolvedSecondParam.GetSystemValue(), context);

            result.CheckDirty();

            return result;
        }

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.WaypointTypeName) };

            Distance.CheckDirty(options);
            HorizontalAngle.CheckDirty(options);

            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode(options) ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ Distance.GetLongHashCode(options) ^ HorizontalAngle.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new WaypointSourceValue(Distance, HorizontalAngle, Name?.Clone(cloneContext));
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Distance), Distance);
            sb.PrintObjProp(n, nameof(HorizontalAngle), HorizontalAngle);
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Distance), Distance);
            sb.PrintShortObjProp(n, nameof(HorizontalAngle), HorizontalAngle);
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Distance), Distance);
            sb.PrintBriefObjProp(n, nameof(HorizontalAngle), HorizontalAngle);
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
        public override string ToHumanizedString()
        {
            if (Name != null && !Name.IsEmpty)
            {
                return $"{Name.NameValue}[{Distance.ToDbgString()}, {HorizontalAngle.ToDbgString()}]";
            }

            return $"#@[{Distance.ToDbgString()}, {HorizontalAngle.ToDbgString()}]";
        }
    }
}
