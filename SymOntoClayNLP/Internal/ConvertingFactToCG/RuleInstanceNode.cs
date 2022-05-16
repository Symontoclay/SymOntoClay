using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public class RuleInstanceNode
    {
        public RuleInstanceNode(RuleInstance fact, ContextOfConverterFactToCG context)
        {
            _fact = fact;
            _context = context;
            _logger = context.Logger;
        }

        private readonly RuleInstance _fact;
        private readonly ContextOfConverterFactToCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
            var result = new ResultOfNode();

            Convert(_fact.PrimaryPart, result);

            throw new NotImplementedException();
        }

        private void Convert(PrimaryRulePart primaryPart, ResultOfNode result)
        {
#if DEBUG
            _logger.Log($"primaryPart = {DebugHelperForRuleInstance.BaseRulePartToString(primaryPart, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var exprResult = LogicalQueryNodeProcessorFactory.Run(primaryPart.Expression, _context);

#if DEBUG
            _logger.Log($"exprResult = {exprResult}");
#endif

            throw new NotImplementedException();
        }
    }
}
