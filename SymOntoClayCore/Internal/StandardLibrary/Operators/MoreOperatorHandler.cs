/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class MoreOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public MoreOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;

        /// <inheritdoc/>
        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            leftOperand = TryResolveFromVar(leftOperand, localCodeExecutionContext);
            rightOperand = TryResolveFromVar(rightOperand, localCodeExecutionContext);

#if DEBUG
            //Log($"leftOperand (after) = {leftOperand}");
            //Log($"rightOperand (after) = {rightOperand}");
#endif

            if (leftOperand.IsSystemNull || rightOperand.IsSystemNull)
            {
                return new NullValue();
            }

            if (leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                var leftOperandValue = leftOperand.GetSystemValue();
                var rightOperandValue = rightOperand.GetSystemValue();

                return CompareSystemValues((double)leftOperandValue, (double)rightOperandValue);
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
                            leftNumVal = ValueConverter.ConvertLogicalValueToNumberValue(leftOperand.AsLogicalValue, _engineContext);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(numKindOfValue), numKindOfValue, null);
                    }

#if DEBUG
                    //Log($"leftNumVal = {leftNumVal}");
#endif

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
                                        return CompareSystemValues((double)leftNumVal.GetSystemValue(), 1);

                                    case "false":
                                        return CompareSystemValues((double)leftNumVal.GetSystemValue(), 0);

                                    default:
                                        {
                                            var eqResult = _fuzzyLogicResolver.Equals(val, leftNumVal, localCodeExecutionContext);

#if DEBUG
                                            //Log($"eqResult = {eqResult}");
#endif

                                            if (eqResult)
                                            {
                                                return new LogicalValue(0);
                                            }

                                            var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(val, localCodeExecutionContext);

#if DEBUG
                                            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                            if (!systemDeffuzzificatedValue.HasValue)
                                            {
                                                return new LogicalValue(false);
                                            }

#if DEBUG
                                            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif
                                            return CompareSystemValues(leftNumVal.SystemValue.Value, systemDeffuzzificatedValue.Value);
                                        }
                                }
                            }

                        case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                            {
                                var val = rightOperand.AsFuzzyLogicNonNumericSequenceValue;

                                var eqResult = _fuzzyLogicResolver.Equals(val, leftNumVal, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return new LogicalValue(0);
                                }

                                var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(val, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return new LogicalValue(false);
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return CompareSystemValues(leftNumVal.SystemValue.Value, systemDeffuzzificatedValue.Value);
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
                            rightNumVal = ValueConverter.ConvertLogicalValueToNumberValue(rightOperand.AsLogicalValue, _engineContext);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(numKindOfValue), numKindOfValue, null);
                    }

#if DEBUG
                    //Log($"rightNumVal = {rightNumVal}");
#endif

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
                                        return CompareSystemValues(1, (double)rightNumVal.GetSystemValue());

                                    case "false":
                                        return CompareSystemValues(0, (double)rightNumVal.GetSystemValue());

                                    default:
                                        {
                                            var eqResult = _fuzzyLogicResolver.Equals(val, rightNumVal, localCodeExecutionContext);

#if DEBUG
                                            //Log($"eqResult = {eqResult}");
#endif

                                            if (eqResult)
                                            {
                                                return new LogicalValue(0);
                                            }

                                            var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(val, localCodeExecutionContext);

#if DEBUG
                                            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                            if (!systemDeffuzzificatedValue.HasValue)
                                            {
                                                return new LogicalValue(false);
                                            }

#if DEBUG
                                            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                            return CompareSystemValues(systemDeffuzzificatedValue.Value, rightNumVal.SystemValue.Value);
                                        }
                                }
                            }

                        case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                            {
                                var val = leftOperand.AsFuzzyLogicNonNumericSequenceValue;

                                var eqResult = _fuzzyLogicResolver.Equals(val, rightNumVal, localCodeExecutionContext);

#if DEBUG
                                //Log($"eqResult = {eqResult}");
#endif

                                if (eqResult)
                                {
                                    return new LogicalValue(0);
                                }

                                var deffuzzificatedValue = _fuzzyLogicResolver.Resolve(val, localCodeExecutionContext);

#if DEBUG
                                //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

                                var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

                                if (!systemDeffuzzificatedValue.HasValue)
                                {
                                    return new LogicalValue(false);
                                }

#if DEBUG
                                //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

                                return CompareSystemValues(systemDeffuzzificatedValue.Value, rightNumVal.SystemValue.Value);
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

        private LogicalValue CompareSystemValues(double leftValue, double rightValue)
        {
            return new LogicalValue(leftValue > rightValue);
        }
    }
}
