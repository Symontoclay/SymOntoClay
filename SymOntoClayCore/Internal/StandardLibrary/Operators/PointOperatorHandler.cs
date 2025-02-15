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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class PointOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public PointOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
        }

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);

            if ((leftOperand.IsHostValue || leftOperand.IsThreadExecutorValue || leftOperand.IsInstanceValue) && rightOperand.IsStrongIdentifierValue/* && rightOperand.AsStrongIdentifierValue.KindOfName == KindOfName.Concept*/)
            {
                var result = new PointRefValue(leftOperand, rightOperand);
                result.CheckDirty();
                return new CallResult(result);
            }

            if (localCodeExecutionContext.Kind == KindOfLocalCodeExecutionContext.AddingFact)
            {
                if (leftOperand.IsRuleInstance && rightOperand.IsStrongIdentifierValue && rightOperand.AsStrongIdentifierValue.KindOfName == KindOfName.Concept)
                {
                    var ruleInstance = leftOperand.AsRuleInstance;

                    if (ruleInstance == localCodeExecutionContext.AddedRuleInstance)
                    {
                        var mutablePartValue = new MutablePartOfRuleInstanceValue(localCodeExecutionContext.MutablePart);

                        var result = new PointRefValue(mutablePartValue, rightOperand);
                        result.CheckDirty();
                        return new CallResult(result);
                    }

                    throw new NotImplementedException("6E572A1D-F103-473A-B433-6ABBD6802375");
                }

                throw new NotImplementedException("C379A65F-6FB0-4AF7-976A-EF645FF56444");
            }

            throw new NotImplementedException("2A2E0115-89CA-466B-B1A9-506E594EFB1A");
        }
    }
}
