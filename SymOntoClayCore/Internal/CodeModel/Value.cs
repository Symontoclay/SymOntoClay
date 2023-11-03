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

using Newtonsoft.Json.Linq;
using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class Value: AnnotatedItem, IEquatable<Value>, IMonitoredMethodIdentifier, IMonitorSerializable
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

        public virtual bool IsTaskValue => false;
        public virtual TaskValue AsTaskValue => null;

        public virtual bool IsProcessInfoValue => false;
        public virtual ProcessInfoValue AsProcessInfoValue => null;

        public virtual bool IsAnnotationValue => false;
        public virtual AnnotationValue AsAnnotationValue => null;

        public virtual bool IsWaypointValue => false;
        public virtual WaypointValue AsWaypointValue => null;

        public virtual bool IsWaypointSourceValue => false;
        public virtual WaypointSourceValue AsWaypointSourceValue => null;

        public virtual bool IsInstanceValue => false;
        public virtual InstanceValue AsInstanceValue => null;

        public virtual bool IsHostValue => false;
        public virtual HostValue AsHostValue => null;

        public virtual bool IsPointRefValue => false;
        public virtual PointRefValue AsPointRefValue => null;

        public virtual bool IsRuleInstance => false;
        public virtual RuleInstance AsRuleInstance => null;

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

        public virtual IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => throw new NotImplementedException();

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
                throw new NotImplementedException();
            }

            return false;
        }

        public virtual bool NullValueEquals()
        {
            return false;
        }

        protected virtual bool ConcreteValueEquals(Value other)
        {
            throw new NotImplementedException();
        }

        public void SetMemberValue(IMonitorLogger logger, StrongIdentifierValue memberName, Value value)
        {
            var kindOfName = memberName.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Var:
                    SetVarValue(logger, memberName, value);
                    break;

                case KindOfName.Concept:
                    SetPropertyValue(logger, memberName, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        protected virtual void SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value)
        {
             throw new NotImplementedException();
        }

        protected virtual void SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            throw new NotImplementedException();
        }

        public virtual void SetValue(IMonitorLogger logger, Value value)
        {
            throw new NotImplementedException();
        }

        public Value GetMemberValue(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
            var kindOfName = memberName.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Var:
                    return GetVarValue(logger, memberName);

                case KindOfName.Concept:
                    return GetPropertyValue(logger, memberName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        protected virtual Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            throw new NotImplementedException();
        }

        protected virtual Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            throw new NotImplementedException();
        }

        public virtual IExecutable GetMethod(IMonitorLogger logger, StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            return null;
        }

        public virtual IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            throw new NotImplementedException();
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
        public virtual string ToLabel()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual object ToMonitorSerializableObject()
        {
            throw new NotImplementedException();
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
