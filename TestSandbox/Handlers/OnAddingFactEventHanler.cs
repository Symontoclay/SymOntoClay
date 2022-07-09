using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class OnAddingFactEventHanler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        public void Run()
        {
            _logger.Log("Begin");

            Case1();

            _logger.Log("End");
        }

        private void Case1()
        {
            OnAddingFact += Handler1;
            OnAddingFact += Handler2;
            OnAddingFact += Handler3;

            RunDelegate(OnAddingFact, null);
        }

        private void RunDelegate(MulticastDelegate onAddingFactEvent, RuleInstance ruleInstance)
        {
            _logger.Log($"onAddingFactEvent.GetInvocationList().Length = {onAddingFactEvent.GetInvocationList().Length}");
        }

        private IAddFactOrRuleResult Handler1(RuleInstance ruleInstance)
        {
            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }

        private IAddFactOrRuleResult Handler2(RuleInstance ruleInstance)
        {
            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }

        private IAddFactOrRuleResult Handler3(RuleInstance ruleInstance)
        {
            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }
    }
}
