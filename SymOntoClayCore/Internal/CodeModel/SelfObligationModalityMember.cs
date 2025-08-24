using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;

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
            throw new NotImplementedException("4889A00B-89EF-425C-88F7-EFDB2D791203");
        }

        /// <inheritdoc/>
        public bool IsHostMethodValue => false;

        /// <inheritdoc/>
        public HostMethodValue AsHostMethodValue => null;


    }
}
