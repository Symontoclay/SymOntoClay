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
        public ValueCallResult Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, IAnnotatedItem annotatedItem, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
#if DEBUG
            Info("7313472E-E3BC-4724-9083-629F1017E0E5", $"leftOperand = {leftOperand}");
            Info("51F20A26-2F61-4129-BB92-DB538A43A7C2", $"callMode = {callMode}");
            Info("AA897067-F227-481B-9AA5-072BA7723338", $"rightOperand = {rightOperand}");
#endif

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
                                            return _varsResolver.SetVarValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext);

                                        default:
                                            return _varsResolver.SetVarValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext);
                                    }
                                }

                            case KindOfName.CommonConcept:
                                return _propertiesResolver.SetPropertyValue(logger, leftIdentifierValue, rightOperand, localCodeExecutionContext, callMode);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfNameOfLeftIdentifierValue), kindOfNameOfLeftIdentifierValue, null);
                        }
                    }

                case KindOfValue.MemberValue:
                case KindOfValue.PointRefValue:
                    return leftOperand.SetValue(logger, rightOperand);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLeftOperand), kindOfLeftOperand, null);
            }

            throw new NotImplementedException("30221B5A-697B-4A3A-8E34-32D5416D75F6");
        }
    }
}
