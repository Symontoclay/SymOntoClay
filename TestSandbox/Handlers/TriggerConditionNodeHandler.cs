using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class TriggerConditionNodeHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case1();

            _logger.Log("End");
        }

        private void Case1()
        {
            var node = new TriggerConditionNode();
            node.Kind = KindOfTriggerConditionNode.Concept;
            node.Name = NameHelper.CreateName("trigger 1");

            _logger.Log($"node = {node}");
            _logger.Log($"node = {node.GetHumanizeDbgString()}");
        }
    }
}
