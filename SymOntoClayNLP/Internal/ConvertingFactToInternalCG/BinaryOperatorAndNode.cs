using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class BinaryOperatorAndNode
    {
        public BinaryOperatorAndNode(LogicalQueryNode op, ContextOfConverterFactToInternalCG context)
        {
            _operator = op;
            _context = context;
            _logger = context.Logger;
        }

        private readonly LogicalQueryNode _operator;
        private readonly ContextOfConverterFactToInternalCG _context;
        private readonly IEntityLogger _logger;

        public ResultOfNode Run()
        {
#if DEBUG
            //_logger.Log($"_operator = {DebugHelperForRuleInstance.ToString(_operator, HumanizedOptions.ShowOnlyMainContent)}");
#endif

            var leftResult = LogicalQueryNodeProcessorFactory.Run(_operator.Left, _context);

#if DEBUG
            //_logger.Log($"leftResult = {leftResult}");
#endif

            var rightResult = LogicalQueryNodeProcessorFactory.Run(_operator.Right, _context);

#if DEBUG
            //_logger.Log($"rightResult = {rightResult}");
#endif

            if(leftResult.KindOfResult == KindOfResultOfNode.ResolveVar && rightResult.KindOfResult == KindOfResultOfNode.ResolveVar)
            {
                return new ResultOfNode { KindOfResult = KindOfResultOfNode.ResolveVar };
            }

            if(leftResult.KindOfResult == KindOfResultOfNode.ResolveVar)
            {
                return rightResult;
            }

            if (rightResult.KindOfResult == KindOfResultOfNode.ResolveVar)
            {
                return leftResult;
            }

            throw new NotImplementedException();
        }
    }
}
