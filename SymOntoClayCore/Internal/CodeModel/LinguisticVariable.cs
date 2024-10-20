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
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LinguisticVariable : CodeItem
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.LinguisticVariable;

        /// <inheritdoc/>
        public override bool IsLinguisticVariable => true;

        /// <inheritdoc/>
        public override LinguisticVariable AsLinguisticVariable => this;

        public RangeValue Range { get; set; } = new RangeValue();
        public LinguisticVariableConstraint Constraint { get; set; } = new LinguisticVariableConstraint();
        public List<FuzzyLogicNonNumericValue> Values { get; set; } = new List<FuzzyLogicNonNumericValue>();
        public List<FuzzyLogicOperator> Operators { get; set; } = new List<FuzzyLogicOperator>();

        public bool IsFitByСonstraintOrDontHasСonstraint(ReasonOfFuzzyLogicResolving reason)
        {
            if (Constraint == null)
            {
                return true;
            }

            if (Constraint.IsEmpty)
            {
                return true;
            }

            return Constraint.isFit(reason);
        }

        public bool IsConstraintNullOrEmpty => Constraint == null || Constraint.IsEmpty;

        public bool IsFitByСonstraint(ReasonOfFuzzyLogicResolving reason)
        {
            if(Constraint == null)
            {
                if(reason == null)
                {
                    return true;
                }

                if(reason.Kind == KindOfReasonOfFuzzyLogicResolving.Unknown)
                {
                    return true;
                }

                return false;
            }

            return Constraint.isFit(reason);
        }

        public bool IsFitByRange(NumberValue x)
        {
            if(Range == null)
            {
                return true;
            }

            return Range.IsFit(x);
        }

        public bool IsFitByRange(double? x)
        {
            if (Range == null)
            {
                return true;
            }

            return Range.IsFit(x);
        }

        public FuzzyLogicOperator GetOperator(StrongIdentifierValue name)
        {
            if(!Operators.Any())
            {
                return null;
            }

            return Operators.SingleOrDefault(p => p.Name == name);
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public LinguisticVariable Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public LinguisticVariable Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LinguisticVariable)context[this];
            }

            var result = new LinguisticVariable();
            context[this] = result;

            result.Range = Range?.Clone(context);
            result.Constraint = Constraint?.Clone(context);
            result.Values = Values?.Select(p => p.Clone(context)).ToList();
            result.Operators = Operators?.Select(p => p.Clone(context)).ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if(!Values.IsNullOrEmpty())
            {
                foreach (var value in Values)
                {
                    value.DiscoverAllAnnotations(result);
                }
            }

            if(!Operators.IsNullOrEmpty())
            {
                foreach(var op in Operators)
                {
                    op.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if(Range != null)
            {
                Range.CheckDirty(options);

                result ^= Range.GetLongHashCode(options);
            }

            if (Constraint != null)
            {
                Constraint.CheckDirty(options);
            }

            if (!Values.IsNullOrEmpty())
            {
                foreach (var value in Values)
                {
                    value.CheckDirty(options);

                    result ^= value.GetLongHashCode(options);
                }
            }

            if (!Operators.IsNullOrEmpty())
            {
                foreach (var op in Operators)
                {
                    op.CheckDirty(options);

                    result ^= op.GetLongHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Range), Range);
            sb.PrintObjProp(n, nameof(Constraint), Constraint);
            sb.PrintObjListProp(n, nameof(Values), Values);
            sb.PrintObjListProp(n, nameof(Operators), Operators);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Range), Range);
            sb.PrintShortObjProp(n, nameof(Constraint), Constraint);
            sb.PrintShortObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Range), Range);
            sb.PrintExisting(n, nameof(Constraint), Constraint);
            sb.PrintExisting(n, nameof(Values), Values);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
