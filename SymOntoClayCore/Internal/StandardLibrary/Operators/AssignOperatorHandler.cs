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
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class AssignOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public AssignOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            var dataResolversFactory = engineContext.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly VarsResolver _varsResolver;

        /// <inheritdoc/>
        public Value Call(IMonitorLogger logger, Value rightOperand, Value leftOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            rightOperand = TryResolveFromVarOrExpr(logger, rightOperand, localCodeExecutionContext);

            var kindOfLeftOperand = leftOperand.KindOfValue;

            switch(kindOfLeftOperand)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var leftIdentifierValue = leftOperand.AsStrongIdentifierValue;

                        var kindOfNameOfLeftIdentifierValue = leftIdentifierValue.KindOfName;

                        switch(kindOfNameOfLeftIdentifierValue)
                        {
                            case KindOfName.Var:
                                {
                                    var kindOfRightOperand = rightOperand.KindOfValue;

                                    switch (kindOfRightOperand)
                                    {
                                        case KindOfValue.StrongIdentifierValue:
                                            _varsResolver.SetVarValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext);
                                            return rightOperand;

                                        default:
                                            _varsResolver.SetVarValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext);
                                            return rightOperand;
                                    }
                                }

                            case KindOfName.Concept:
                                {
#if DEBUG
                                    Info("9E484258-2A5E-4389-92CB-36BD6114BBBC", $"leftOperand = {leftOperand}");
                                    Info("E981235C-808D-4F6B-AF59-97213B85599D", $"callMode = {callMode}");
#endif

                                    throw new NotImplementedException("4E4695A2-4FAC-4584-9890-21E6D624E9DF");
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfNameOfLeftIdentifierValue), kindOfNameOfLeftIdentifierValue, null);
                        }
                    }

                case KindOfValue.PointRefValue:
                    leftOperand.SetValue(logger, rightOperand);
                    return rightOperand;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLeftOperand), kindOfLeftOperand, null);
            }

            throw new NotImplementedException("30221B5A-697B-4A3A-8E34-32D5416D75F6");
        }
    }
}
