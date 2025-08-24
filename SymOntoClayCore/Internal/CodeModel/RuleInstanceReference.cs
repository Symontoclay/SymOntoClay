using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

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

        /// <inheritdoc/>
        public override bool IsRuleInstanceReference => true;

        /// <inheritdoc/>
        public override RuleInstanceReference AsRuleInstanceReference => this;

        public RuleInstance CurrentRuleInstance { get; private set; }

        /// <inheritdoc/>
        public override IMember GetMember(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
#if DEBUG
            logger.Info("7E8BD5EE-C5C9-4306-8AB0-0DC14AE622EA", $"memberName = {memberName}");
#endif

            throw new NotImplementedException("95EC3694-7C8D-49D2-A8C5-BA16729E576A");
        }

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
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
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
