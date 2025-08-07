using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class HostMethodValue : Value, IMember
    {
        public HostMethodValue(StrongIdentifierValue methodName) 
        {
            MethodName = methodName;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.HostMethodValue;

        /// <inheritdoc/>
        public override bool IsHostMethodValue => true;

        /// <inheritdoc/>
        public override HostMethodValue AsHostMethodValue => this;

        /// <inheritdoc/>
        public KindOfMember KindOfMember => KindOfMember.HostMethod;

        /// <inheritdoc/>
        public ValueCallResult GetValue(IMonitorLogger logger)
        {
            throw new NotImplementedException("2A89A5E7-4610-4C37-9A2E-3DB36E2BD551");
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("2D6D0125-F3A7-4934-8748-D16788DE1B1C");
        }

        public StrongIdentifierValue MethodName { get; private set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.HostWeight;
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

            var result = new HostMethodValue(MethodName);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(MethodName), MethodName);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(MethodName), MethodName);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(MethodName), MethodName);

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

        private string NToHumanizedString()
        {
            return $"ref: @@Host.{MethodName.ToHumanizedLabel()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
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
