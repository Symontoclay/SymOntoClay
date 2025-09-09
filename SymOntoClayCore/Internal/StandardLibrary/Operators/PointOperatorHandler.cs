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
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class PointOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public PointOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
        }

        /// <inheritdoc/>
        public ValueCallResult Call(IMonitorLogger logger, KindOfValueConversion kindOfValueConversion, Value leftOperand, Value rightOperand, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
#if DEBUG
            //Info("099033EE-C837-4106-B08B-136AE6CA7497", $"leftOperand = {leftOperand}");
            //Info("2ACF6A1F-431D-43FB-984D-D62298CF4B44", $"rightOperand = {rightOperand}");
            //Info("8C57ECA2-FE45-4263-9003-6AB0765C7862", $"callMode = {callMode}");
            //Info("D049609C-EC5D-4A88-A208-E3693797C380", $"kindOfValueConversion = {kindOfValueConversion}");
#endif

            if ((leftOperand.IsHostValue || leftOperand.IsThreadExecutorValue || leftOperand.IsInstanceValue) && rightOperand.IsStrongIdentifierValue)
            {
                var member = leftOperand.GetMember(logger, rightOperand.AsStrongIdentifierValue.ForResolving);

#if DEBUG
                //Info("A1A2448C-C179-4408-97DD-CFC68FF34CB2", $"member = {member}");
#endif

                var value = new MemberValue(member);
                return new ValueCallResult(value);
            }
            
            if (leftOperand.IsRuleInstanceReference && rightOperand.IsStrongIdentifierValue && rightOperand.AsStrongIdentifierValue.KindOfName == KindOfName.CommonConcept)
            {
                var member = leftOperand.GetMember(logger, rightOperand.AsStrongIdentifierValue.ForResolving);

#if DEBUG
                //Info("E8D46E7A-CC32-48C0-9ED2-F1BBA48BA1B1", $"member = {member}");
#endif

                var value = new MemberValue(member);
                return new ValueCallResult(value);
            }

            throw new NotImplementedException("2A2E0115-89CA-466B-B1A9-506E594EFB1A");
        }
    }
}
