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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
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
        public Value Call(IList<Value> paramsList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsList[0];
            var rightOperand = paramsList[1];
            var anotation = paramsList[2];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
        
        /// <inheritdoc/>
        public Value Call(IDictionary<string, Value> paramsDict, Value anotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsDict[_leftOperandKey];
            var rightOperand = paramsDict[_rightOperandKey];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
    }
}
