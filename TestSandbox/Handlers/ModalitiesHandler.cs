using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class ModalitiesHandler
    {
        public ModalitiesHandler()
        {
            _engineContext = TstEngineContextHelper.CreateAndInitContext().EngineContext;
        }

        private readonly IEngineContext _engineContext;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } :}";

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact (before) = {DebugHelperForRuleInstance.ToString(fact)}");

            fact.ObligationModality = LogicalValue.TrueValue;
            fact.SelfObligationModality = LogicalValue.FalseValue;

            //_logger.Log($"fact = {fact}");

            _logger.Log($"fact (after) = {DebugHelperForRuleInstance.ToString(fact)}");

            _logger.Log("End");
        }
    }
}
