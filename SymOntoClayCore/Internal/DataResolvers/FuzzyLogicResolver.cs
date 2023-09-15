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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class FuzzyLogicResolver : BaseResolver
    {
        public FuzzyLogicResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _inheritanceResolver = dataResolversFactory.GetInheritanceResolver();
            _toSystemBoolResolver = dataResolversFactory.GetToSystemBoolResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private readonly ToSystemBoolResolver _toSystemBoolResolver;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly SynonymsResolver _synonymsResolver;

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public NumberValue Resolve(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(value, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, Value value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, Value value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(_numberValueLinearResolver.CanBeResolved(value))
            {
                return _numberValueLinearResolver.Resolve(value, localCodeExecutionContext, options);
            }

            if(value.IsStrongIdentifierValue)
            {
                return Resolve(value.AsStrongIdentifierValue, reason, localCodeExecutionContext, options);
            }

            if(value.IsFuzzyLogicNonNumericSequenceValue)
            {
                return Resolve(value.AsFuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);
            }

            throw new NotImplementedException();
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, null, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var fuzzyValue = targetItem.Handler.Defuzzificate();

            return fuzzyValue;
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, null, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators).Select(p => p.Handler);

            var fuzzyValue = targetItem.Handler.Defuzzificate(operatorsList).SystemValue.Value;

            return new NumberValue(fuzzyValue);
        }
        
        public bool Equals(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(value1 == null && value2 == null)
            {
                return true;
            }

            if (value1.KindOfValue == KindOfValue.NullValue && value2.KindOfValue == KindOfValue.NullValue)
            {
                return true;
            }

            if ((value1.IsNumberValue || value1.IsLogicalValue) && (value2.IsNumberValue || value2.IsLogicalValue))
            {
                var sysValue1 = value1.GetSystemValue();
                var sysValue2 = value2.GetSystemValue();

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            if (value1.IsStrongIdentifierValue && value2.IsStrongIdentifierValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                if (value1StrongIdentifierValue == value2StrongIdentifierValue)
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Equals(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Equals(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsNumberValue || value1.IsLogicalValue || value2.IsNumberValue || value2.IsLogicalValue)
            {
                if(value1.IsStrongIdentifierValue || value2.IsStrongIdentifierValue)
                {
                    StrongIdentifierValue conceptValue = null;
                    Value numberValue = null;

                    if(value1.IsStrongIdentifierValue)
                    {
                        conceptValue = value1.AsStrongIdentifierValue;
                        numberValue = value2;
                    }
                    else
                    {
                        conceptValue = value2.AsStrongIdentifierValue;
                        numberValue = value1;
                    }

                    return Equals(conceptValue, _numberValueLinearResolver.Resolve(numberValue, localCodeExecutionContext), reason, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue || value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence = null;
                    Value numberValue = null;

                    if(value1.IsFuzzyLogicNonNumericSequenceValue)
                    {
                        fuzzyLogicNonNumericSequence = value1.AsFuzzyLogicNonNumericSequenceValue;
                        numberValue = value2;
                    }
                    else
                    {
                        fuzzyLogicNonNumericSequence = value2.AsFuzzyLogicNonNumericSequenceValue;
                        numberValue = value1;
                    }

                    return Equals(fuzzyLogicNonNumericSequence, _numberValueLinearResolver.Resolve(numberValue, localCodeExecutionContext), reason, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if((value1.IsStrongIdentifierValue || value1.IsFuzzyLogicNonNumericSequenceValue) && (value2.IsStrongIdentifierValue || value2.IsFuzzyLogicNonNumericSequenceValue))
            {
                StrongIdentifierValue conceptValue = null;
                FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence = null;

                if (value1.IsStrongIdentifierValue)
                {
                    conceptValue = value1.AsStrongIdentifierValue;
                    fuzzyLogicNonNumericSequence = value2.AsFuzzyLogicNonNumericSequenceValue;
                }
                else
                {
                    conceptValue = value2.AsStrongIdentifierValue;
                    fuzzyLogicNonNumericSequence = value1.AsFuzzyLogicNonNumericSequenceValue;
                }

                var conceptNumberValue = Resolve(conceptValue, reason, localCodeExecutionContext, options);

                return Equals(fuzzyLogicNonNumericSequence, conceptNumberValue, reason, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, value, reason, localCodeExecutionContext, options);

            if(targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, value, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators);

            foreach (var op in operatorsList)
            {
                fuzzyValue = op.Handler.SystemCall(fuzzyValue);
            }

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool More(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (value1 == null && value2 == null)
            {
                return false;
            }

            if (value1.KindOfValue == KindOfValue.NullValue && value2.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            var numberValueLinearResolver = _numberValueLinearResolver;

            if (numberValueLinearResolver.CanBeResolved(value1) && numberValueLinearResolver.CanBeResolved(value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

                if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                {
                    return leftSystemNullaleValue.Value > rightSystemNullaleValue.Value;
                }
                else
                {
                    if (!leftSystemNullaleValue.HasValue && !rightSystemNullaleValue.HasValue)
                    {
                        return false;
                    }
                }

                return false;
            }

            if (value1.IsStrongIdentifierValue && value2.IsStrongIdentifierValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                if (value1StrongIdentifierValue == value2StrongIdentifierValue)
                {
                    return false;
                }

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return More(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return false;
                }

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return More(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return More(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return More(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return More(leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return More(leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if(numberValueLinearResolver.CanBeResolved(value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                if(value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return More(value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if(value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return More(value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }

            if (value1.KindOfValue == KindOfValue.NullValue && value2.KindOfValue == KindOfValue.NullValue)
            {
                return true;
            }

            var numberValueLinearResolver = _numberValueLinearResolver;

            if (numberValueLinearResolver.CanBeResolved(value1) && numberValueLinearResolver.CanBeResolved(value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

                if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                {
                    return leftSystemNullaleValue.Value >= rightSystemNullaleValue.Value;
                }
                else
                {
                    if (!leftSystemNullaleValue.HasValue && !rightSystemNullaleValue.HasValue)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (value1.IsStrongIdentifierValue && value2.IsStrongIdentifierValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                if (value1StrongIdentifierValue == value2StrongIdentifierValue)
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return MoreOrEqual(leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return MoreOrEqual(leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if (numberValueLinearResolver.CanBeResolved(value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return MoreOrEqual(value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return MoreOrEqual(value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool Less(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (value1 == null && value2 == null)
            {
                return false;
            }

            if (value1.KindOfValue == KindOfValue.NullValue && value2.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            var numberValueLinearResolver = _numberValueLinearResolver;

            if (numberValueLinearResolver.CanBeResolved(value1) && numberValueLinearResolver.CanBeResolved(value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

                if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                {
                    return leftSystemNullaleValue.Value < rightSystemNullaleValue.Value;
                }
                else
                {
                    if (!leftSystemNullaleValue.HasValue && !rightSystemNullaleValue.HasValue)
                    {
                        return false;
                    }
                }

                return false;
            }

            if (value1.IsStrongIdentifierValue && value2.IsStrongIdentifierValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                if (value1StrongIdentifierValue == value2StrongIdentifierValue)
                {
                    return false;
                }

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Less(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return false;
                }

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Less(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Less(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Less(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return Less(leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return Less(leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if (numberValueLinearResolver.CanBeResolved(value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return Less(value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return Less(value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }

            if (value1.KindOfValue == KindOfValue.NullValue && value2.KindOfValue == KindOfValue.NullValue)
            {
                return true;
            }

            var numberValueLinearResolver = _numberValueLinearResolver;

            if (numberValueLinearResolver.CanBeResolved(value1) && numberValueLinearResolver.CanBeResolved(value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

                if (leftSystemNullaleValue.HasValue && rightSystemNullaleValue.HasValue)
                {
                    return leftSystemNullaleValue.Value <= rightSystemNullaleValue.Value;
                }
                else
                {
                    if (!leftSystemNullaleValue.HasValue && !rightSystemNullaleValue.HasValue)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (value1.IsStrongIdentifierValue && value2.IsStrongIdentifierValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                if (value1StrongIdentifierValue == value2StrongIdentifierValue)
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return LessOrEqual(leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return LessOrEqual(leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            if (numberValueLinearResolver.CanBeResolved(value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return LessOrEqual(value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return LessOrEqual(value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        private bool FuzzyNumericValueToSystemBool(IMonitorLogger logger, double fuzzyValue)
        {
            return _toSystemBoolResolver.Resolve(fuzzyValue);
        }

        private List<FuzzyLogicOperator> GetFuzzyLogicOperators(IMonitorLogger logger, LinguisticVariable linguisticVariable, IEnumerable<StrongIdentifierValue> operatorsIdentifiers)
        {
            if(operatorsIdentifiers.IsNullOrEmpty())
            {
                return new List<FuzzyLogicOperator>();
            }

            var result = new List<FuzzyLogicOperator>();

            var globalFuzzyLogicStorage = _context.Storage.GlobalStorage.FuzzyLogicStorage;

            foreach (var op in operatorsIdentifiers)
            {
                var item = linguisticVariable.GetOperator(op);

                if(item == null)
                {
                    item = globalFuzzyLogicStorage.GetDefaultOperator(op);

                    if(item == null)
                    {
                        throw new Exception($"Unexpected fuzzy logic operator `{op.NameValue}`!");
                    }
                }

                result.Add(item);
            }

            return result;
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValue(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            if(reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance)
            {
                optionsForInheritanceResolver.SkipRealSearching = true;
                optionsForInheritanceResolver.AddSelf = false;
            }

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(name, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

            if (!filteredList.Any())
            {
                return null;
            }

            if((reason == null || reason.Kind != KindOfReasonOfFuzzyLogicResolving.Inheritance) && value != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByRange(value)).ToList();

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            if(reason != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByСonstraintOrDontHasСonstraint(reason)).ToList();

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            return GetTargetFuzzyLogicNonNumericValueFromList(filteredList, reason);
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValueFromList(IMonitorLogger logger, List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> list, ReasonOfFuzzyLogicResolving reason)
        {
            if (!list.Any())
            {
                return null;
            }

            if (list.Count == 1)
            {
                return list.FirstOrDefault()?.ResultItem;
            }

            if(list.Any(p => p.ResultItem.Parent.IsFitByСonstraint(reason)) && list.Any(p => p.ResultItem.Parent.IsConstraintNullOrEmpty))
            {
                list = list.Where(p => p.ResultItem.Parent.IsFitByСonstraint(reason)).ToList();

                if (!list.Any())
                {
                    return null;
                }

                if (list.Count == 1)
                {
                    return list.FirstOrDefault()?.ResultItem;
                }
            }

            var minLengthOfRange = list.Min(p => p.ResultItem.Parent.Range.Length);

            var targetItem = list.FirstOrDefault(p => p.ResultItem.Parent.Range.Length == minLengthOfRange)?.ResultItem;

            return targetItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> GetRawList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var synonymsList = _synonymsResolver.GetSynonyms(name, storagesList);

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();

            var itemsList = NGetRawList(name, storagesList, weightedInheritanceItems);

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawList(synonym, storagesList, weightedInheritanceItems);

                if (!itemsList.IsNullOrEmpty())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> NGetRawList(IMonitorLogger logger, StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            if (!storagesList.Any())
            {
                return new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();
            }

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();

            foreach (var storageItem in storagesList)
            {
                var itemsList = storageItem.Storage.FuzzyLogicStorage.GetNonNumericValuesDirectly(name, weightedInheritanceItems);

                if (!itemsList.Any())
                {
                    continue;
                }

                var distance = storageItem.Priority;
                var storage = storageItem.Storage;

                foreach (var item in itemsList)
                {
                    result.Add(new WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>(item, distance, storage));
                }
            }

            return result;
        }
    }
}
