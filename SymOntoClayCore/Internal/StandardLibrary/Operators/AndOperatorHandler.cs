/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class AndOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public AndOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
        }

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVarOrExpr(logger, rightOperand, localCodeExecutionContext);

            if (leftOperand.IsSystemNull || rightOperand.IsSystemNull)
            {
                return new CallResult(NullValue.Instance);
            }

            if (leftOperand.IsLogicalValue && rightOperand.IsLogicalValue)
            {
                return new CallResult(LogicalValue.And(leftOperand.AsLogicalValue, rightOperand.AsLogicalValue));
            }

            throw new NotImplementedException("029A59D2-4CD5-4156-9BB3-11EAF85FE18E");
        }
    }
}
