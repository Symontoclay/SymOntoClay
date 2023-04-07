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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
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
        public Value Call(Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            leftOperand = TryResolveFromVarOrExpr(leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVarOrExpr(rightOperand, localCodeExecutionContext);

            if (leftOperand.IsSystemNull && rightOperand.IsSystemNull)
            {
                return new LogicalValue(1);
            }

            if(leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                return CompareSystemValues((double)leftOperand.GetSystemValue(), (double)rightOperand.GetSystemValue());
            }

            if(((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue)) || ((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue)))
            {
                if((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue))
                {
                    return CompareWithFuzzyLogic(leftOperand, rightOperand, localCodeExecutionContext);
                }

                if((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue))
                {
                    return CompareWithFuzzyLogic(rightOperand, leftOperand, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if ((leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue) && (leftOperand.IsStrongIdentifierValue || leftOperand.IsInstanceValue))
            {
                return GetInheritanceRank(leftOperand, rightOperand, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        private LogicalValue CompareWithFuzzyLogic(Value numOperand, Value fuzzyOperand, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            NumberValue numVal = null;

            var numKindOfValue = numOperand.KindOfValue;

            switch (numKindOfValue)
            {
                case KindOfValue.NumberValue:
                    numVal = numOperand.AsNumberValue;
                    break;

                case KindOfValue.LogicalValue:
                    numVal = ValueConverter.ConvertLogicalValueToNumberValue(numOperand.AsLogicalValue, _engineContext);
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
                                return CompareSystemValues((double)numVal.GetSystemValue(), 1);

                            case "false":
                                return CompareSystemValues((double)numVal.GetSystemValue(), 0);

                            default:
                                return new LogicalValue(_fuzzyLogicResolver.Equals(val, numVal, localCodeExecutionContext));
                        }
                    }

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    return new LogicalValue(_fuzzyLogicResolver.Equals(fuzzyOperand.AsFuzzyLogicNonNumericSequenceValue, numVal, localCodeExecutionContext));

                default:
                    throw new ArgumentOutOfRangeException(nameof(fuzzyKindOfValue), fuzzyKindOfValue, null);
            }

            throw new NotImplementedException();
        }

        private LogicalValue CompareSystemValues(double leftValue, double rightValue)
        {
            return new LogicalValue(leftValue == rightValue);
        }

        private Value GetInheritanceRank(Value leftOperand, Value rightOperand, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var subName = _strongIdentifierLinearResolver.Resolve(leftOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(rightOperand, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

            return _inheritanceResolver.GetInheritanceRank(subName, superName, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());
        }
    }
}
