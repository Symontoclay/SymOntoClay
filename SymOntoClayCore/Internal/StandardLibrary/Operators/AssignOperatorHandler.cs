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
    public class AssignOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public AssignOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            var dataResolversFactory = engineContext.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
            _propertiesResolver = dataResolversFactory.GetPropertiesResolver();
        }

        private readonly VarsResolver _varsResolver;
        private readonly PropertiesResolver _propertiesResolver;

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value rightOperand, Value leftOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
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
                                            return new CallResult(rightOperand);

                                        default:
                                            _varsResolver.SetVarValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext);
                                            return new CallResult(rightOperand);
                                    }
                                }

                            case KindOfName.Concept:
                                {
#if DEBUG
                                    Info("9E484258-2A5E-4389-92CB-36BD6114BBBC", $"leftOperand = {leftOperand}");
                                    Info("E981235C-808D-4F6B-AF59-97213B85599D", $"callMode = {callMode}");
                                    Info("489E4FD3-8244-42B5-B73C-D5B03B9C2165", $"rightOperand = {rightOperand}");
#endif

                                    var property = _propertiesResolver.Resolve(logger, leftIdentifierValue, localCodeExecutionContext);

#if DEBUG
                                    Info("1B9FF0A5-D834-409F-A555-4E447E8C71DE", $"property = {property}");
#endif

                                    switch(callMode)
                                    {
                                        case CallMode.PreConstructor:
                                            if (property == null)
                                            {
                                                throw new NotImplementedException("C1DA392F-6F4F-4DB3-8DFA-AE49E3CD0354");
                                            }
                                            else
                                            {
                                                property.SetValue(logger, rightOperand);
                                                return new CallResult(rightOperand);
                                            }
                                            
                                        case CallMode.Default:
                                            if (property == null)
                                            {
                                                throw new NotImplementedException("05DDD3F9-E954-464B-B60C-1C520F45CC5C");
                                            }
                                            else
                                            {
                                                throw new NotImplementedException("4E4695A2-4FAC-4584-9890-21E6D624E9DF");
                                            }                                            

                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(callMode), callMode, null);
                                    }
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfNameOfLeftIdentifierValue), kindOfNameOfLeftIdentifierValue, null);
                        }
                    }

                case KindOfValue.PointRefValue:
                    leftOperand.SetValue(logger, rightOperand);
                    return new CallResult(rightOperand);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLeftOperand), kindOfLeftOperand, null);
            }

            throw new NotImplementedException("30221B5A-697B-4A3A-8E34-32D5416D75F6");
        }
    }
}
