using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SelfObligationModalityMember: IMember, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString
    {
        public SelfObligationModalityMember(RuleInstanceReference ruleInstanceReference)
        {
            _ruleInstanceReference = ruleInstanceReference;
        }

        /// <inheritdoc/>
        public KindOfMember KindOfMember => KindOfMember.SelfObligationModality;

        private RuleInstanceReference _ruleInstanceReference;

        /// <inheritdoc/>
        public ValueCallResult GetValue(IMonitorLogger logger)
        {
            return new ValueCallResult(_ruleInstanceReference.CurrentRuleInstance.SelfObligationModality);
        }

        /// <inheritdoc/>
        public ValueCallResult SetValue(IMonitorLogger logger, Value value)
        {
            _ruleInstanceReference.TryCloneCurrentRuleInstance();

            _ruleInstanceReference.CurrentRuleInstance.SelfObligationModality = value;

            return new ValueCallResult(value);
        }

        /// <inheritdoc/>
        public bool IsHostMethodValue => false;

        /// <inheritdoc/>
        public HostMethodValue AsHostMethodValue => null;

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(_ruleInstanceReference), _ruleInstanceReference);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(_ruleInstanceReference), _ruleInstanceReference);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(_ruleInstanceReference), _ruleInstanceReference);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedString(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedLabel(options);
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedLabel(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedLabel(options);
        }

        private string NToHumanizedLabel(DebugHelperOptions options)
        {
            options ??= DebugHelperOptions.FromHumanizedOptions(HumanizedOptions.ShowAll);

            return $"so: {_ruleInstanceReference?.CurrentRuleInstance?.SelfObligationModality}";
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedLabel(null)
            };
        }
    }
}
