using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ObligationModalityMember: IMember, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString
    {
        public ObligationModalityMember(RuleInstanceReference ruleInstanceReference)
        {
            _ruleInstanceReference = ruleInstanceReference;
        }

        /// <inheritdoc/>
        public KindOfMember KindOfMember => KindOfMember.ObligationModality;

        private RuleInstanceReference _ruleInstanceReference;

        /// <inheritdoc/>
        public ValueCallResult GetValue(IMonitorLogger logger)
        {
            return new ValueCallResult(_ruleInstanceReference.CurrentRuleInstance.ObligationModality);
        }

        /// <inheritdoc/>
        public ValueCallResult SetValue(IMonitorLogger logger, Value value)
        {
            throw new NotImplementedException("6DE24260-A7B2-4D51-8E42-EED195EEEF79");
        }

        /// <inheritdoc/>
        public bool IsHostMethodValue => false;

        /// <inheritdoc/>
        public HostMethodValue AsHostMethodValue => null;


    }
}
