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
    public class AddOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public AddOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
        }

        /// <inheritdoc/>
        public Value Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVarOrExpr(logger, rightOperand, localCodeExecutionContext);

            if (leftOperand.IsNullValue || rightOperand.IsNullValue)
            {
                return new NullValue();
            }

            if (leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                var leftOperandValue = leftOperand.GetSystemValue();
                var rightOperandValue = rightOperand.GetSystemValue();

                if (leftOperandValue == null || rightOperandValue == null)
                {
                    return new NullValue();
                }

                return new NumberValue((double)leftOperandValue + (double)rightOperandValue);
            }

            if(leftOperand.IsStringValue || rightOperand.IsStringValue)
            {
                return new StringValue(leftOperand.ToSystemString() + rightOperand.ToSystemString());
            }

            throw new NotImplementedException();
        }
    }
}
