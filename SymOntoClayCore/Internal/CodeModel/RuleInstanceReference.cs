using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstanceReference: Value
    {
        public RuleInstanceReference(RuleInstance originalRuleInstance)
        {
            CurrentRuleInstance = originalRuleInstance;
        }

        private RuleInstanceReference()
        {
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RuleInstanceReference;

        public RuleInstance CurrentRuleInstance { get; private set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException("4835EE9A-329B-4F7E-A5FA-F8FC2C00013C");
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("AE2BF91D-686D-4A8E-B224-D8C4A8E028E3");
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
        public RuleInstanceReference Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public RuleInstanceReference Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RuleInstanceReference)context[this];
            }

            var result = new RuleInstanceReference();
            context[this] = result;

            result.CurrentRuleInstance = CurrentRuleInstance?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return CurrentRuleInstance?.ToHumanizedLabel(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return CurrentRuleInstance?.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return CurrentRuleInstance?.ToLabel(logger);
        }
    }
}
