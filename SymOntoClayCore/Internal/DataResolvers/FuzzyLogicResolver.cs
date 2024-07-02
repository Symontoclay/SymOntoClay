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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return Resolve(logger, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, Value value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, Value value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            if(_numberValueLinearResolver.CanBeResolved(logger, value))
            {
                return _numberValueLinearResolver.Resolve(logger, value, localCodeExecutionContext, options);
            }

            if(value.IsStrongIdentifierValue)
            {
                return Resolve(logger, value.AsStrongIdentifierValue, reason, localCodeExecutionContext, options);
            }

            if(value.IsFuzzyLogicNonNumericSequenceValue)
            {
                return Resolve(logger, value.AsFuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);
            }

            throw new NotImplementedException("46E63A8F-1691-40C0-A7A5-F5B5A5CB27BB");
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(logger, name, null, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var fuzzyValue = targetItem.Handler.Defuzzificate(logger);

            return fuzzyValue;
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(logger, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(logger, fuzzyLogicNonNumericSequence.NonNumericValue, null, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var operatorsList = GetFuzzyLogicOperators(logger, targetItem.Parent, fuzzyLogicNonNumericSequence.Operators).Select(p => p.Handler);

            var fuzzyValue = targetItem.Handler.Defuzzificate(logger, operatorsList).SystemValue.Value;

            return new NumberValue(fuzzyValue);
        }
        
        public bool Equals(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, value1, value2, reason, localCodeExecutionContext, _defaultOptions);
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

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Equals(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Equals(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
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

                    return Equals(logger, conceptValue, _numberValueLinearResolver.Resolve(logger, numberValue, localCodeExecutionContext), reason, localCodeExecutionContext);
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

                    return Equals(logger, fuzzyLogicNonNumericSequence, _numberValueLinearResolver.Resolve(logger, numberValue, localCodeExecutionContext), reason, localCodeExecutionContext);
                }

                throw new NotImplementedException("96D6F92E-9385-4DA4-A675-F88CFEA20ACD");
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

                var conceptNumberValue = Resolve(logger, conceptValue, reason, localCodeExecutionContext, options);

                return Equals(logger, fuzzyLogicNonNumericSequence, conceptNumberValue, reason, localCodeExecutionContext);
            }

            throw new NotImplementedException("A73D7410-1995-4D7B-BE42-30FE251EA309");
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(logger, name, value, reason, localCodeExecutionContext, options);

            if(targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(logger, value);

            return FuzzyNumericValueToSystemBool(logger, fuzzyValue);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(logger, fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var targetItem = GetTargetFuzzyLogicNonNumericValue(logger, fuzzyLogicNonNumericSequence.NonNumericValue, value, reason, localCodeExecutionContext, options);

            if (targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(logger, value);

            var operatorsList = GetFuzzyLogicOperators(logger, targetItem.Parent, fuzzyLogicNonNumericSequence.Operators);

            foreach (var op in operatorsList)
            {
                fuzzyValue = op.Handler.SystemCall(logger, fuzzyValue);
            }

            return FuzzyNumericValueToSystemBool(logger, fuzzyValue);
        }

        public bool More(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value1, value2, reason, localCodeExecutionContext, _defaultOptions);
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

            if (numberValueLinearResolver.CanBeResolved(logger, value1) && numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

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

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return More(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return false;
                }

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return More(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return More(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return More(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return More(logger, leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return More(logger, leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException("FB3EE582-7E18-4F50-A2C1-675856413179");
            }

            if(numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

                if(value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return More(logger, value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if(value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return More(logger, value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException("A1EF47E3-3B4E-4478-947B-C3D2BB4ADC27");
            }

            throw new NotImplementedException("7FE4D5C7-BC81-4897-A3D7-71870AEF513F");
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(logger, value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value1, value2, reason, localCodeExecutionContext, _defaultOptions);
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

            if (numberValueLinearResolver.CanBeResolved(logger, value1) && numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

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

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return MoreOrEqual(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return MoreOrEqual(logger, leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return MoreOrEqual(logger, leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException("78E66BDC-9803-4D02-9058-CADAED22367E");
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return MoreOrEqual(logger, value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return MoreOrEqual(logger, value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException("2AEE0E0D-7C26-465E-8575-8A7CC32357B5");
            }

            throw new NotImplementedException("78E6B769-D908-422A-8AAF-36A05EB364E4");
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(logger, value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool Less(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value1, value2, reason, localCodeExecutionContext, _defaultOptions);
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

            if (numberValueLinearResolver.CanBeResolved(logger, value1) && numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

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

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Less(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return false;
                }

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Less(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return Less(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return Less(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return Less(logger, leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return Less(logger, leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException("9ED90E5F-CCAA-4F14-BB5B-975E9DC9F289");
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return Less(logger, value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return Less(logger, value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException("DCEACD63-90C2-4B45-84E9-8FFE8B570975");
            }

            throw new NotImplementedException("8DB87C33-595B-4E4C-BF7C-5957BAE59EA1");
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(logger, value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, Value value1, Value value2, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value1, value2, reason, localCodeExecutionContext, _defaultOptions);
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

            if (numberValueLinearResolver.CanBeResolved(logger, value1) && numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

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

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                if (value1FuzzyLogicNonNumericSequenceValue.Equals(value2FuzzyLogicNonNumericSequenceValue))
                {
                    return true;
                }

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(logger, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(logger, value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(logger, value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

                return LessOrEqual(logger, value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(logger, value1, localCodeExecutionContext);

                if (value2.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return LessOrEqual(logger, leftNumberValue, value2StrongIdentifierValue, reason, localCodeExecutionContext);
                }

                if (value2.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return LessOrEqual(logger, leftNumberValue, value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext);
                }

                throw new NotImplementedException("016D7024-6357-4A0F-9CE3-793AC7CFC43C");
            }

            if (numberValueLinearResolver.CanBeResolved(logger, value2))
            {
                var rightNumberValue = numberValueLinearResolver.Resolve(logger, value2, localCodeExecutionContext);

                if (value1.IsStrongIdentifierValue)
                {
                    var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                    return LessOrEqual(logger, value2StrongIdentifierValue, rightNumberValue, localCodeExecutionContext);
                }

                if (value1.IsFuzzyLogicNonNumericSequenceValue)
                {
                    var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                    return LessOrEqual(logger, value2FuzzyLogicNonNumericSequenceValue, rightNumberValue, localCodeExecutionContext);
                }

                throw new NotImplementedException("05B8F580-1F97-45CD-A4E2-B33CB37A5B15");
            }

            throw new NotImplementedException("1A9694BD-B554-4CE1-94BB-7492C6EE98EE");
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, name, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, name, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(logger, value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(IMonitorLogger logger, NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(logger, fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(logger, fuzzyLogicNonNumericSequence, localCodeExecutionContext);

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        private bool FuzzyNumericValueToSystemBool(IMonitorLogger logger, double fuzzyValue)
        {
            return _toSystemBoolResolver.Resolve(logger, fuzzyValue);
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
                    item = globalFuzzyLogicStorage.GetDefaultOperator(logger, op);

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

            var storagesList = GetStoragesList(logger, storage, KindOfStoragesList.CodeItems);

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            if(reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance)
            {
                optionsForInheritanceResolver.SkipRealSearching = true;
                optionsForInheritanceResolver.AddSelf = false;
            }

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(logger, localCodeExecutionContext, optionsForInheritanceResolver);

            var rawList = GetRawList(logger, name, storagesList, weightedInheritanceItems);

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(logger, rawList);

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

            return GetTargetFuzzyLogicNonNumericValueFromList(logger, filteredList, reason);
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
            var synonymsList = _synonymsResolver.GetSynonyms(logger, name, storagesList);

            var result = new List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>>();

            var itemsList = NGetRawList(logger, name, storagesList, weightedInheritanceItems);

            if (!itemsList.IsNullOrEmpty())
            {
                result.AddRange(itemsList);
            }

            foreach (var synonym in synonymsList)
            {
                itemsList = NGetRawList(logger, synonym, storagesList, weightedInheritanceItems);

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
                var itemsList = storageItem.Storage.FuzzyLogicStorage.GetNonNumericValuesDirectly(logger, name, weightedInheritanceItems);

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
