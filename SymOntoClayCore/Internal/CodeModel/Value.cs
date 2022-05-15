/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class Value: AnnotatedItem
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

        public virtual bool IsRuleInstanceValue => false;
        public virtual RuleInstanceValue AsRuleInstanceValue => null;

        public virtual bool IsLogicalSearchResultValue => false;
        public virtual LogicalSearchResultValue AsLogicalSearchResultValue => null;

        public virtual bool IsLogicalQueryOperationValue => false;
        public virtual LogicalQueryOperationValue AsLogicalQueryOperationValue => null;

        public virtual bool IsFuzzyLogicNonNumericSequenceValue => false;
        public virtual FuzzyLogicNonNumericSequenceValue AsFuzzyLogicNonNumericSequenceValue => null;

        public virtual bool IsRangeValue => false;
        public virtual RangeValue AsRangeValue => null;

        public virtual bool IsFunctionValue => false;
        public virtual FunctionValue AsFunctionValue => null;

        public virtual bool IsErrorValue => false;
        public virtual ErrorValue AsErrorValue => null;

        public virtual bool IsActionInstanceValue => false;
        public virtual ActionInstanceValue AsActionInstanceValue => null;

        public virtual bool IsConditionalEntitySourceValue => false;
        public virtual ConditionalEntitySourceValue AsConditionalEntitySourceValue => null;

        public virtual bool IsConditionalEntityValue => false;
        public virtual ConditionalEntityValue AsConditionalEntityValue => null;

        public virtual bool IsSystemNull { get; }

        public virtual IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => throw new NotImplementedException();

        public abstract object GetSystemValue();

        public abstract string ToSystemString();

        public abstract string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll);

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Value CloneValue()
        {
            var cloneContext = new Dictionary<object, object>();
            return CloneValue(cloneContext);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="cloneContext">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract Value CloneValue(Dictionary<object, object> cloneContext);

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
