using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class OnAddingFactEventHanler
    {
        public OnAddingFactEventHanler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContextWithoutAppFiles().EngineContext;
        }

        private static readonly IEntityLogger _logger = new LoggerImpementation();
        private readonly IEngineContext _engineContext;

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

            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";
            var ruleInstance = Parse(factStr);

            var result = RunDelegate(OnAddingFact, ruleInstance);

            _logger.Log($"result = {result}");
        }

        private IAddFactOrRuleResult RunDelegate(MulticastDelegate onAddingFactEvent, RuleInstance ruleInstance)
        {
            _logger.Log($"onAddingFactEvent.GetInvocationList().Length = {onAddingFactEvent.GetInvocationList().Length}");

            foreach (var item in onAddingFactEvent.GetInvocationList())
            {
                var rawResultOfCall = item.DynamicInvoke(ruleInstance);

                //_logger.Log($"rawResultOfCall = {rawResultOfCall}");

                if(rawResultOfCall == null)
                {
                    continue;
                }

                var resultOfCall = (IAddFactOrRuleResult)rawResultOfCall;

                _logger.Log($"resultOfCall = {resultOfCall}");

                if(resultOfCall.KindOfResult == KindOfAddFactOrRuleResult.Reject)
                {
                    return resultOfCall;
                }
            }

            throw new NotImplementedException();
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
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.FalseValue }
            };
        }

        private IAddFactOrRuleResult Handler3(RuleInstance ruleInstance)
        {
            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept
            };
        }

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }
    }
}
