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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
        }

        private readonly ToSystemBoolResolver _toSystemBoolResolver;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        public NumberValue Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"reason = {reason}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, null, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var fuzzyValue = targetItem.Handler.Defuzzificate();

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return fuzzyValue;
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public NumberValue Resolve(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, null, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return new NumberValue(null);
            }

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators).Select(p => p.Handler);

#if DEBUG
            //Log($"operatorsList.Count = {operatorsList.Count}");
#endif

            var fuzzyValue = targetItem.Handler.Defuzzificate(operatorsList).SystemValue.Value;

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return new NumberValue(fuzzyValue);
        }
        
        public bool Equals(Value value1, Value value2, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"value1 = {value1}");
            //Log($"value2 = {value2}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                    //Log($"conceptValue = {conceptValue}");
                    //Log($"numberValue = {numberValue}");
#endif

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

#if DEBUG
                    //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
                    //Log($"numberValue = {numberValue}");
#endif

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

#if DEBUG
                //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
                //Log($"conceptValue = {conceptValue}");
#endif

                var conceptNumberValue = Resolve(conceptValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"conceptNumberValue = {conceptNumberValue}");
#endif

                return Equals(fuzzyLogicNonNumericSequence, conceptNumberValue, reason, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(name, value, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if(targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Equals(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Equals(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"fuzzyLogicNonNumericSequence = {fuzzyLogicNonNumericSequence}");
            //Log($"value = {value}");
#endif

            var targetItem = GetTargetFuzzyLogicNonNumericValue(fuzzyLogicNonNumericSequence.NonNumericValue, value, reason, localCodeExecutionContext, options);

#if DEBUG
            //Log($"targetItem = {targetItem}");
#endif

            if (targetItem == null)
            {
                return false;
            }

            var fuzzyValue = targetItem.Handler.SystemCall(value);

#if DEBUG
            //Log($"fuzzyValue = {fuzzyValue}");
#endif

            var operatorsList = GetFuzzyLogicOperators(targetItem.Parent, fuzzyLogicNonNumericSequence.Operators);

#if DEBUG
            //Log($"operatorsList.Count = {operatorsList.Count}");
#endif

            foreach (var op in operatorsList)
            {
#if DEBUG
                //Log($"op = {op}");
#endif

                fuzzyValue = op.Handler.SystemCall(fuzzyValue);
            }

#if DEBUG
            //Log($"fuzzyValue (after) = {fuzzyValue}");
#endif

            return FuzzyNumericValueToSystemBool(fuzzyValue);
        }

        public bool More(Value value1, Value value2, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"value1 = {value1}");
            //Log($"value2 = {value2}");
#endif

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

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                //Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                //Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return More(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return More(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if(value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return More(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
#endif

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

#if DEBUG
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

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

        public bool More(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(NumberValue value, StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool More(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value > value.SystemValue.Value;
        }

        public bool More(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return More(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool More(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value > systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(Value value1, Value value2, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"value1 = {value1}");
            //Log($"value2 = {value2}");
#endif

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

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                //Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                //Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return MoreOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return MoreOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return MoreOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
#endif

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

#if DEBUG
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

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

        public bool MoreOrEqual(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(NumberValue value, StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool MoreOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value >= value.SystemValue.Value;
        }

        public bool MoreOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return MoreOrEqual(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool MoreOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value >= systemDeffuzzificatedValue.Value;
        }

        public bool Less(Value value1, Value value2, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"value1 = {value1}");
            //Log($"value2 = {value2}");
#endif

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

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                //Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                //Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return Less(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return Less(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return Less(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
#endif

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

#if DEBUG
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

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

        public bool Less(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(NumberValue value, StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool Less(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value < value.SystemValue.Value;
        }

        public bool Less(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Less(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool Less(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return false;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value < systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(Value value1, Value value2, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value1, value2, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value1, value2, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(Value value1, Value value2, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"value1 = {value1}");
            //Log($"value2 = {value2}");
#endif

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

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

                var leftSystemNullaleValue = leftNumberValue.SystemValue;
                var rightSystemNullaleValue = rightNumberValue.SystemValue;

#if DEBUG
                //Log($"leftSystemNullaleValue = {leftSystemNullaleValue}");
                //Log($"rightSystemNullaleValue = {rightSystemNullaleValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

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

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return LessOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsStrongIdentifierValue && value2.IsFuzzyLogicNonNumericSequenceValue)
            {
                var value1StrongIdentifierValue = value1.AsStrongIdentifierValue;
                var value2FuzzyLogicNonNumericSequenceValue = value2.AsFuzzyLogicNonNumericSequenceValue;

                var value2NumberValue = Resolve(value2FuzzyLogicNonNumericSequenceValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return LessOrEqual(value1StrongIdentifierValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (value1.IsFuzzyLogicNonNumericSequenceValue && value2.IsStrongIdentifierValue)
            {
                var value1FuzzyLogicNonNumericSequenceValue = value1.AsFuzzyLogicNonNumericSequenceValue;
                var value2StrongIdentifierValue = value2.AsStrongIdentifierValue;

                var value2NumberValue = Resolve(value2StrongIdentifierValue, reason, localCodeExecutionContext, options);

#if DEBUG
                //Log($"value2NumberValue = {value2NumberValue}");
#endif

                return LessOrEqual(value1FuzzyLogicNonNumericSequenceValue, value2NumberValue, reason, localCodeExecutionContext);
            }

            if (numberValueLinearResolver.CanBeResolved(value1))
            {
                var leftNumberValue = numberValueLinearResolver.Resolve(value1, localCodeExecutionContext);

#if DEBUG
                //Log($"leftNumberValue = {leftNumberValue}");
#endif

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

#if DEBUG
                //Log($"rightNumberValue = {rightNumberValue}");
#endif

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

        public bool LessOrEqual(StrongIdentifierValue name, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(name, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(name, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(NumberValue value, StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, name, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, name, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(NumberValue value, StrongIdentifierValue name, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(name, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(name, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        public bool LessOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(fuzzyLogicNonNumericSequence, value, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(fuzzyLogicNonNumericSequence, value, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return systemDeffuzzificatedValue.Value <= value.SystemValue.Value;
        }

        public bool LessOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, fuzzyLogicNonNumericSequence, null, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return LessOrEqual(value, fuzzyLogicNonNumericSequence, reason, localCodeExecutionContext, _defaultOptions);
        }

        public bool LessOrEqual(NumberValue value, FuzzyLogicNonNumericSequenceValue fuzzyLogicNonNumericSequence, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var eqResult = Equals(fuzzyLogicNonNumericSequence, value, localCodeExecutionContext);

#if DEBUG
            //Log($"eqResult = {eqResult}");
#endif
            if (eqResult)
            {
                return true;
            }

            var deffuzzificatedValue = Resolve(fuzzyLogicNonNumericSequence, localCodeExecutionContext);

#if DEBUG
            //Log($"deffuzzificatedValue = {deffuzzificatedValue}");
#endif

            var systemDeffuzzificatedValue = deffuzzificatedValue.SystemValue;

            if (!systemDeffuzzificatedValue.HasValue)
            {
                return false;
            }

#if DEBUG
            //Log($"systemDeffuzzificatedValue = {systemDeffuzzificatedValue}");
#endif

            return value.SystemValue.Value <= systemDeffuzzificatedValue.Value;
        }

        private bool FuzzyNumericValueToSystemBool(double fuzzyValue)
        {
            return _toSystemBoolResolver.Resolve(fuzzyValue);
        }

        private List<FuzzyLogicOperator> GetFuzzyLogicOperators(LinguisticVariable linguisticVariable, IEnumerable<StrongIdentifierValue> operatorsIdentifiers)
        {
            if(operatorsIdentifiers.IsNullOrEmpty())
            {
                return new List<FuzzyLogicOperator>();
            }

            var result = new List<FuzzyLogicOperator>();

            var globalFuzzyLogicStorage = _context.Storage.GlobalStorage.FuzzyLogicStorage;

            foreach (var op in operatorsIdentifiers)
            {
#if DEBUG
                //Log($"op = {op}");
#endif

                var item = linguisticVariable.GetOperator(op);

#if DEBUG
                //Log($"item = {item}");
#endif

                if(item == null)
                {
                    item = globalFuzzyLogicStorage.GetDefaultOperator(op);

#if DEBUG
                    //Log($"item (2) = {item}");
#endif

                    if(item == null)
                    {
                        throw new Exception($"Unexpected fuzzy logic operator `{op.NameValue}`!");
                    }
                }

                result.Add(item);
            }

            return result;
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValue(StrongIdentifierValue name, NumberValue value, ReasonOfFuzzyLogicResolving reason, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"name = {name}");
            //Log($"value = {value}");
            //Log($"reason = {reason}");
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach (var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Key = {tmpStorage.Key}; tmpStorage.Value.Kind = '{tmpStorage.Value.Kind}'");
            //}
#endif

            var optionsForInheritanceResolver = options.Clone();
            optionsForInheritanceResolver.AddSelf = true;

            if(reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance)
            {
#if DEBUG
                //Log("^%^%^%^%^%^% reason != null && reason.Kind == KindOfReasonOfFuzzyLogicResolving.Inheritance");
#endif

                optionsForInheritanceResolver.SkipRealSearching = true;
                optionsForInheritanceResolver.AddSelf = false;
            }

            var weightedInheritanceItems = _inheritanceResolver.GetWeightedInheritanceItems(localCodeExecutionContext, optionsForInheritanceResolver);

#if DEBUG
            //Log($"weightedInheritanceItems = {weightedInheritanceItems.WriteListToString()}");
#endif

            var rawList = GetRawList(name, storagesList, weightedInheritanceItems);

#if DEBUG
            //Log($"rawList = {rawList.WriteListToString()}");
#endif

            if (!rawList.Any())
            {
                return null;
            }

            var filteredList = Filter(rawList);

#if DEBUG
            //Log($"filteredList = {filteredList.WriteListToString()}");
#endif

            if (!filteredList.Any())
            {
                return null;
            }

            if((reason == null || reason.Kind != KindOfReasonOfFuzzyLogicResolving.Inheritance) && value != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByRange(value)).ToList();

#if DEBUG
                //Log($"filteredList (2) = {filteredList.WriteListToString()}");
#endif

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            if(reason != null)
            {
                filteredList = filteredList.Where(p => p.ResultItem.Parent.IsFitByСonstraintOrDontHasСonstraint(reason)).ToList();

#if DEBUG
                //Log($"filteredList (3) = {filteredList.WriteListToString()}");
#endif

                if (!filteredList.Any())
                {
                    return null;
                }
            }

            return GetTargetFuzzyLogicNonNumericValueFromList(filteredList, reason);
        }

        private FuzzyLogicNonNumericValue GetTargetFuzzyLogicNonNumericValueFromList(List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> list, ReasonOfFuzzyLogicResolving reason)
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

#if DEBUG
            //Log($"minLengthOfRange = {minLengthOfRange}");
#endif

            var targetItem = list.FirstOrDefault(p => p.ResultItem.Parent.Range.Length == minLengthOfRange)?.ResultItem;

            return targetItem;
        }

        private List<WeightedInheritanceResultItemWithStorageInfo<FuzzyLogicNonNumericValue>> GetRawList(StrongIdentifierValue name, List<StorageUsingOptions> storagesList, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

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
