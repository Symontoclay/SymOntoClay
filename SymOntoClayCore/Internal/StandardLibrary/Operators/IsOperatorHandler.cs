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
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class IsOperatorHandler: BaseOperatorHandler, IBinaryOperatorHandler
    {
        public IsOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext, CallMode callMode)
        {
            leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVarOrExpr(logger, rightOperand, localCodeExecutionContext);

            if (leftOperand.IsSystemNull && rightOperand.IsSystemNull)
            {
                return new CallResult(LogicalValue.TrueValue);
            }

            if(leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                return CompareSystemValues(logger, (double)leftOperand.GetSystemValue(), (double)rightOperand.GetSystemValue());
            }

            if(((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue)) || ((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue)))
            {
                if((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue))
                {
                    return CompareWithFuzzyLogic(logger, leftOperand, rightOperand, localCodeExecutionContext);
                }

                if((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue))
                {
                    return CompareWithFuzzyLogic(logger, rightOperand, leftOperand, localCodeExecutionContext);
                }

                throw new NotImplementedException("D8C168A2-DB52-4869-BE01-C78E795611E3");
            }

            if ((leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue) && (leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue))
            {
                return GetInheritanceRank(logger, leftOperand, rightOperand, localCodeExecutionContext);
            }

            throw new NotImplementedException("30F5F633-3254-454C-9CD9-1DD619001B9A");
        }

        private CallResult CompareWithFuzzyLogic(IMonitorLogger logger, Value numOperand, Value fuzzyOperand, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            NumberValue numVal = null;

            var numKindOfValue = numOperand.KindOfValue;

            switch (numKindOfValue)
            {
                case KindOfValue.NumberValue:
                    numVal = numOperand.AsNumberValue;
                    break;

                case KindOfValue.LogicalValue:
                    numVal = ValueConverter.ConvertLogicalValueToNumberValue(logger, numOperand.AsLogicalValue, _engineContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(numKindOfValue), numKindOfValue, null);
            }

            var fuzzyKindOfValue = fuzzyOperand.KindOfValue;

            switch (fuzzyKindOfValue)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var val = fuzzyOperand.AsStrongIdentifierValue;

                        var normalizedNameValue = val.NormalizedNameValue;

                        switch (normalizedNameValue)
                        {
                            case "true":
                                return CompareSystemValues(logger, (double)numVal.GetSystemValue(), 1);

                            case "false":
                                return CompareSystemValues(logger, (double)numVal.GetSystemValue(), 0);

                            default:
                                return new CallResult(new LogicalValue(_fuzzyLogicResolver.Equals(logger, val, numVal, localCodeExecutionContext)));
                        }
                    }

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    return new CallResult(new LogicalValue(_fuzzyLogicResolver.Equals(logger, fuzzyOperand.AsFuzzyLogicNonNumericSequenceValue, numVal, localCodeExecutionContext)));

                default:
                    throw new ArgumentOutOfRangeException(nameof(fuzzyKindOfValue), fuzzyKindOfValue, null);
            }

            throw new NotImplementedException("1A2F8243-F4DB-44BB-B34E-701A32D3BFF3");
        }

        private CallResult CompareSystemValues(IMonitorLogger logger, double leftValue, double rightValue)
        {
            return new CallResult(new LogicalValue(leftValue == rightValue));
        }

        private CallResult GetInheritanceRank(IMonitorLogger logger, Value leftOperand, Value rightOperand, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var subName = _strongIdentifierLinearResolver.Resolve(logger, leftOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(logger, rightOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            return new CallResult(_inheritanceResolver.GetInheritanceRank(logger, subName, superName, localCodeExecutionContext, ResolverOptions.GetDefaultOptions()));
        }
    }
}
