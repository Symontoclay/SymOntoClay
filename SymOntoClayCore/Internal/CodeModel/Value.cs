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

using NLog;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class Value: AnnotatedItem, IEquatable<Value>, IMonitoredMethodIdentifier, IMonitoredObject
    {
        public abstract KindOfValue KindOfValue { get; }

        public virtual bool IsNullValue => false;
        public virtual NullValue AsNullValue => null;

        public virtual bool IsLogicalValue => false;
        public virtual LogicalValue AsLogicalValue => null;

        public virtual bool IsNumberValue => false;
        public virtual NumberValue AsNumberValue => null;

        public virtual bool IsStringValue => false;
        public virtual StringValue AsStringValue => null;

        public virtual bool IsStrongIdentifierValue => false;
        public virtual StrongIdentifierValue AsStrongIdentifierValue => null;

        public virtual bool IsThreadExecutorValue => false;
        public virtual ThreadExecutorValue AsThreadExecutorValue => null;

        public virtual bool IsProcessInfoValue => false;
        public virtual ProcessInfoValue AsProcessInfoValue => null;

        public virtual bool IsWaypointValue => false;
        public virtual WaypointValue AsWaypointValue => null;

        public virtual bool IsWaypointSourceValue => false;
        public virtual WaypointSourceValue AsWaypointSourceValue => null;

        public virtual bool IsInstanceValue => false;
        public virtual InstanceValue AsInstanceValue => null;

        public virtual bool IsMemberValue => false;
        public virtual MemberValue AsMemberValue => null;

        public virtual bool IsHostValue => false;
        public virtual HostValue AsHostValue => null;

        public virtual bool IsHostMethodValue => false;
        public virtual HostMethodValue AsHostMethodValue => null;

        public virtual bool IsRuleInstance => false;
        public virtual RuleInstance AsRuleInstance => null;

        public virtual bool IsRuleInstanceReference => false;
        public virtual RuleInstanceReference AsRuleInstanceReference => null;

        public virtual bool IsMutablePartOfRuleInstanceValue => false;
        public virtual MutablePartOfRuleInstanceValue AsMutablePartOfRuleInstanceValue => null;

        public virtual bool IsLogicalSearchResultValue => false;
        public virtual LogicalSearchResultValue AsLogicalSearchResultValue => null;

        public virtual bool IsLogicalQueryOperationValue => false;
        public virtual LogicalQueryOperationValue AsLogicalQueryOperationValue => null;

        public virtual bool IsFuzzyLogicNonNumericSequenceValue => false;
        public virtual FuzzyLogicNonNumericSequenceValue AsFuzzyLogicNonNumericSequenceValue => null;

        public virtual bool IsRangeValue => false;
        public virtual RangeValue AsRangeValue => null;

        public virtual bool IsErrorValue => false;
        public virtual ErrorValue AsErrorValue => null;

        public virtual bool IsActionInstanceValue => false;
        public virtual ActionInstanceValue AsActionInstanceValue => null;

        public virtual bool IsConditionalEntitySourceValue => false;
        public virtual ConditionalEntitySourceValue AsConditionalEntitySourceValue => null;

        public virtual bool IsConditionalEntityValue => false;
        public virtual ConditionalEntityValue AsConditionalEntityValue => null;

        public virtual bool IsEntityValue => false;
        public virtual EntityValue AsEntityValue => null;

        public virtual bool IsLogicalModalityExpressionValue => false;
        public virtual LogicalModalityExpressionValue AsLogicalModalityExpressionValue => null;

        public virtual bool IsCodeItem => false;
        public virtual CodeItem AsCodeItem => null;

        public virtual bool IsSequenceValue => false;
        public virtual SequenceValue AsSequenceValue => null;

        public virtual bool IsSystemNull => false;

        public virtual IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => throw new NotImplementedException($"8D5DE0AD-6FDE-41EB-B60E-2630F229C4C0: {GetType().FullName}");

        public abstract object GetSystemValue();

        public abstract string ToSystemString();

        /// <inheritdoc/>
        public bool Equals(Value other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.IsNullValue)
            {
                return NullValueEquals();
            }

            if (KindOfValue == other.KindOfValue)
            {
                return ConcreteValueEquals(other);
            }

            if ((KindOfValue == KindOfValue.NumberValue && other.KindOfValue == KindOfValue.LogicalValue) ||
                (KindOfValue == KindOfValue.LogicalValue && other.KindOfValue == KindOfValue.NumberValue))
            {
                throw new NotImplementedException("36D48D29-E00F-4D0B-AC8A-DA23FCFADBA4");
            }

            return false;
        }

        public virtual bool NullValueEquals()
        {
            return false;
        }

        protected virtual bool ConcreteValueEquals(Value other)
        {
            throw new NotImplementedException($"A3C123A9-471D-4477-B5FC-D93A74A1B710: {GetType().FullName}");
        }

        public ValueCallResult SetMemberValue(IMonitorLogger logger, StrongIdentifierValue memberName, Value value)
        {
            var kindOfName = memberName.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Var:
                    return SetVarValue(logger, memberName, value);

                case KindOfName.CommonConcept:
                    return SetPropertyValue(logger, memberName, value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        protected virtual ValueCallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value)
        {
             throw new NotImplementedException($"0B65BA6A-37C0-4C99-B911-A9450744EA17: {GetType().FullName}");
        }

        protected virtual ValueCallResult SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            throw new NotImplementedException($"AA45CD1D-2E04-440F-88A2-37907994FB76: {GetType().FullName}");
        }

        public virtual ValueCallResult SetValue(IMonitorLogger logger, Value value)
        {
            throw new NotImplementedException($"E4583867-F52A-443A-A926-7BBDD35F826D: {GetType().FullName}");
        }

        public virtual IMember GetMember(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
            throw new NotImplementedException($"D0150E58-94E9-46C0-BA19-5C8022763D24: {GetType().FullName}");
        }

        public Value GetMemberValue(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
            var kindOfName = memberName.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Var:
                    return GetVarValue(logger, memberName);

                case KindOfName.CommonConcept:
                    return GetPropertyValue(logger, memberName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        protected virtual Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            throw new NotImplementedException($"A63D5CA7-1443-49C0-86CE-5CDF5D7C8F2A: {GetType().FullName}");
        }

        protected virtual Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            throw new NotImplementedException($"2A9BFB77-BC79-4742-BB4B-1C9FDADDF25D: {GetType().FullName}");
        }

        public virtual IExecutable GetMethod(IMonitorLogger logger, StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            return null;
        }

        public virtual IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            throw new NotImplementedException($"58DE8BF6-83CB-4965-9979-056EF82E48E8: {GetType().FullName}");
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Value CloneValue()
        {
            var context = new Dictionary<object, object>();
            return CloneValue(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public abstract Value CloneValue(Dictionary<object, object> context);

        /// <inheritdoc/>
        public abstract MonitoredHumanizedLabel ToLabel(IMonitorLogger logger);

        /// <inheritdoc/>
        public virtual object ToMonitorSerializableObject(IMonitorLogger logger)
        {
            throw new NotImplementedException($"CED1BBCD-46B2-46C6-BFA6-9E876D27845E: {GetType().FullName}");
        }

        /// <inheritdoc/>
        string IMonitoredHumanizedObject.ToHumanizedString(IMonitorLogger logger)
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
