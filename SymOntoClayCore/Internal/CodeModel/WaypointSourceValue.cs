using SymOntoClay.Core.Internal.CodeExecution;
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

            return new WaypointValue((float)(double)resolvedFirstParam.GetSystemValue(), (float)(double)resolvedSecondParam.GetSystemValue(), context);
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            Distance.CheckDirty();
            HorizontalAngle.CheckDirty();

            return base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode() ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ Distance.GetLongHashCode() ^ HorizontalAngle.GetLongHashCode();
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
            if (Name != null && !Name.IsEmpty)
            {
                return $"{Name.NameValue}[{Distance.ToDbgString()}, {HorizontalAngle.ToDbgString()}]";
            }

            return $"#@[{Distance.ToDbgString()}, {HorizontalAngle.ToDbgString()}]";
        }
    }
}
