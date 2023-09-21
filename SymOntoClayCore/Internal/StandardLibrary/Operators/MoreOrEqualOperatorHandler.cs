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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class MoreOrEqualOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public MoreOrEqualOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;

        /// <inheritdoc/>
        public Value Call(IMonitorLogger logger, Value leftOperand, Value rightOperand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVarOrExpr(logger, rightOperand, localCodeExecutionContext);

            if (leftOperand.IsSystemNull || rightOperand.IsSystemNull)
            {
                return new NullValue();
            }

            if (leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                var leftOperandValue = leftOperand.GetSystemValue();
                var rightOperandValue = rightOperand.GetSystemValue();

                return CompareSystemValues(logger, (double)leftOperandValue, (double)rightOperandValue);
            }

            if (((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue)) || ((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue)))
            {
                if ((leftOperand.IsNumberValue || leftOperand.IsLogicalValue) && (rightOperand.IsStrongIdentifierValue || rightOperand.IsFuzzyLogicNonNumericSequenceValue))
                {
                    NumberValue leftNumVal = null;

                    var numKindOfValue = leftOperand.KindOfValue;

                    switch (numKindOfValue)
                    {
                        case KindOfValue.NumberValue:
                            leftNumVal = leftOperand.AsNumberValue;
                            break;

                        case KindOfValue.LogicalValue:
                            leftNumVal = ValueConverter.ConvertLogicalValueToNumberValue(logger, leftOperand.AsLogicalValue, _engineContext);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(numKindOfValue), numKindOfValue, null);
                    }

                    var fuzzyKindOfValue = rightOperand.KindOfValue;

                    switch (fuzzyKindOfValue)
                    {
                        case KindOfValue.StrongIdentifierValue:
                            {
                                var val = rightOperand.AsStrongIdentifierValue;

                                var normalizedNameValue = val.NormalizedNameValue;

                                switch (normalizedNameValue)
                                {
                                    case "true":
                                        return CompareSystemValues(logger, (double)leftNumVal.GetSystemValue(), 1);

                                    case "false":
                                        return CompareSystemValues(logger, (double)leftNumVal.GetSystemValue(), 0);

                                    default:
                                        {
                                            var eqResult = _fuzzyLogicResolver.Equals(logger, val, leftNumVal, localCodeExecutionContext);

                                            if(eqResult)
                                            {
                                                return new LogicalValue(1);
                                            }

                                            var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(logger, val, localCodeExecutionContext);

                                            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                            if (!systemDeffuzzificatedValue.HasValue)
                                            {
                                                return new LogicalValue(false);
                                            }

                                            return CompareSystemValues(logger, leftNumVal.SystemValue.Value, systemDeffuzzificatedValue.Value);
                                        }
                                }
                            }

                        case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                            {
                                var val = rightOperand.AsFuzzyLogicNonNumericSequenceValue;

                                var eqResult = _fuzzyLogicResolver.Equals(logger, val, leftNumVal, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return new LogicalValue(1);
                                }

                                var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(logger, val, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return new LogicalValue(false);
                                }

                                return CompareSystemValues(logger, leftNumVal.SystemValue.Value, systemDeffuzzificatedValue.Value);
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(fuzzyKindOfValue), fuzzyKindOfValue, null);
                    }

                    throw new NotImplementedException();
                }

                if ((leftOperand.IsStrongIdentifierValue || leftOperand.IsFuzzyLogicNonNumericSequenceValue) && (rightOperand.IsNumberValue || rightOperand.IsLogicalValue))
                {
                    NumberValue rightNumVal = null;

                    var numKindOfValue = rightOperand.KindOfValue;

                    switch (numKindOfValue)
                    {
                        case KindOfValue.NumberValue:
                            rightNumVal = rightOperand.AsNumberValue;
                            break;

                        case KindOfValue.LogicalValue:
                            rightNumVal = ValueConverter.ConvertLogicalValueToNumberValue(logger, rightOperand.AsLogicalValue, _engineContext);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(numKindOfValue), numKindOfValue, null);
                    }

                    var fuzzyKindOfValue = leftOperand.KindOfValue;

                    switch (fuzzyKindOfValue)
                    {
                        case KindOfValue.StrongIdentifierValue:
                            {
                                var val = leftOperand.AsStrongIdentifierValue;

                                var normalizedNameValue = val.NormalizedNameValue;

                                switch (normalizedNameValue)
                                {
                                    case "true":
                                        return CompareSystemValues(logger, 1, (double)rightNumVal.GetSystemValue());

                                    case "false":
                                        return CompareSystemValues(logger, 0, (double)rightNumVal.GetSystemValue());

                                    default:
                                        {
                                            var eqResult = _fuzzyLogicResolver.Equals(logger, val, rightNumVal, localCodeExecutionContext);

                                            if (eqResult)
                                            {
                                                return new LogicalValue(1);
                                            }

                                            var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(logger, val, localCodeExecutionContext);

                                            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                            if (!systemDeffuzzificatedValue.HasValue)
                                            {
                                                return new LogicalValue(false);
                                            }

                                            return CompareSystemValues(logger, systemDeffuzzificatedValue.Value, rightNumVal.SystemValue.Value);
                                        }
                                }
                            }

                        case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                            {
                                var val = leftOperand.AsFuzzyLogicNonNumericSequenceValue;

                                var eqResult = _fuzzyLogicResolver.Equals(logger, val, rightNumVal, localCodeExecutionContext);

                                if (eqResult)
                                {
                                    return new LogicalValue(1);
                                }

                                var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(logger, val, localCodeExecutionContext);

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return new LogicalValue(false);
                                }

                                return CompareSystemValues(logger, systemDeffuzzificatedValue.Value, rightNumVal.SystemValue.Value);
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(fuzzyKindOfValue), fuzzyKindOfValue, null);
                    }

                    throw new NotImplementedException();
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private LogicalValue CompareSystemValues(IMonitorLogger logger, double leftValue, double rightValue)
        {
            return new LogicalValue(leftValue >= rightValue);
        }
    }
}
