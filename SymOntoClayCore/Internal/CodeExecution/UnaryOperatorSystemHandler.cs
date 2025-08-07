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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class UnaryOperatorSystemHandler : ISystemHandler
    {
        public UnaryOperatorSystemHandler(IUnaryOperatorHandler operatorHandler)
        {
            _operandKey = "operand";
            _operatorHandler = operatorHandler;
        }

        private readonly string _operandKey;
        private readonly IUnaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public ValueCallResult Call(IMonitorLogger logger, IList<Value> paramsList, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            var operand = paramsList[0];
            
            return _operatorHandler.Call(logger, operand, annotatedItem, localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public ValueCallResult Call(IMonitorLogger logger, IDictionary<string, Value> paramsDict, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            var operand = paramsDict[_operandKey];

            return _operatorHandler.Call(logger, operand, annotatedItem, localCodeExecutionContext);
        }
    }
}
