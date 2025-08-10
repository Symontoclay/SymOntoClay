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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class BinaryOperatorSystemHandler: ISystemHandler
    {
        public BinaryOperatorSystemHandler(IBinaryOperatorHandler operatorHandler)
        {
            _leftOperandKey = "leftOperand";
            _rightOperandKey = "rightOperand";
            _operatorHandler = operatorHandler;
        }

        private readonly string _leftOperandKey;
        private readonly string _rightOperandKey;
        private readonly IBinaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public ValueCallResult Call(IMonitorLogger logger, IList<Value> paramsList, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
#if DEBUG
            //logger.Info("7FD4A78D-850D-47A9-A84D-94E4BDB270DE", $"paramsList.Count = {paramsList.Count}");
            //logger.Info("4EF055E4-1B5E-472A-8F15-162271DD69AE", $"paramsList = {paramsList.WriteListToString()}");
#endif

            var leftOperand = paramsList[0];
            var rightOperand = paramsList[1];
            
            return _operatorHandler.Call(logger, leftOperand, rightOperand, annotatedItem, localCodeExecutionContext, callMode);
        }
        
        /// <inheritdoc/>
        public ValueCallResult Call(IMonitorLogger logger, IDictionary<string, Value> paramsDict, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            var leftOperand = paramsDict[_leftOperandKey];
            var rightOperand = paramsDict[_rightOperandKey];

            return _operatorHandler.Call(logger, leftOperand, rightOperand, annotatedItem, localCodeExecutionContext, callMode);
        }
    }
}
