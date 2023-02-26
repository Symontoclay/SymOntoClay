/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.Dot;
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

            var rightResult = LogicalQueryNodeProcessorFactory.Run(_operator.Right, _context);

#if DEBUG
            //_logger.Log($"leftResult = {leftResult}");
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

#if DEBUG
            //var dotStr = DotConverter.ConvertToString(_context.ConceptualGraph);
            //_logger.Log($"dotStr = {dotStr}");
#endif

            return new ResultOfNode { KindOfResult = KindOfResultOfNode.ProcessAndOperator };
        }
    }
}
