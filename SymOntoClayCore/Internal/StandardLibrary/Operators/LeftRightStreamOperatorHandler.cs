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

using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class LeftRightStreamOperatorHandler: BaseOperatorHandler, IBinaryOperatorHandler
    {
        public LeftRightStreamOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _channelsResolver = dataResolversFactory.GetChannelsResolver();
            _valueResolvingHelper = dataResolversFactory.GetValueResolvingHelper();
        }

        private readonly IEngineContext _engineContext;
        private readonly ChannelsResolver _channelsResolver;
        private readonly ValueResolvingHelper _valueResolvingHelper;

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
#if DEBUG
            Info("C1C8D33E-AC4C-4C0A-9328-6A320AD57291", $"leftOperand = {leftOperand}");
            Info("FC6F4EC4-4CE9-45EE-97B7-898DD8D83051", $"rightOperand = {rightOperand}");
#endif

            Value valueFromSource = null;

            var leftOperandKindOfValue = leftOperand.KindOfValue;

#if DEBUG
            Info("D349C868-0F5D-467E-98AB-697E1DA8478F", $"leftOperandKindOfValue = {leftOperandKindOfValue}");
#endif

            var leftOperandCallResult = _valueResolvingHelper.TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);

#if DEBUG
            Info("5AF75343-8D96-48AA-B2F1-EA5A77D8BA7C", $"leftOperandCallResult = {leftOperandCallResult}");
#endif

            var leftOperandCallResultKindOfResult = leftOperandCallResult.KindOfResult;

#if DEBUG
            Info("003D3104-A009-4284-8F8D-C729AC409849", $"leftOperandCallResultKindOfResult = {leftOperandCallResultKindOfResult}");
#endif

            switch (leftOperandCallResultKindOfResult)
            {
                case KindOfCallResult.Value:
                    valueFromSource = leftOperandCallResult.Value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(leftOperandCallResultKindOfResult), leftOperandCallResultKindOfResult, null);
            }

            var rightOperandKindOfValue = rightOperand.KindOfValue;

#if DEBUG
            Info("78B92AA4-7527-49E2-8A9D-278C57524F9D", $"rightOperandKindOfValue = {rightOperandKindOfValue}");
#endif

            switch(rightOperandKindOfValue)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var identifier = rightOperand.AsStrongIdentifierValue;

                        if (identifier.KindOfName == KindOfName.Channel)
                        {
                            var channel = _channelsResolver.GetChannel(logger, identifier, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

                            return new CallResult(channel.Handler.Write(logger, valueFromSource, localCodeExecutionContext));
                        }
                        else
                        {
                            throw new Exception("Right operand should be a channel.");
                        }
                    }

                default:
                    throw new Exception("Right operand should be a channel.");
            }        
        }
    }
}
