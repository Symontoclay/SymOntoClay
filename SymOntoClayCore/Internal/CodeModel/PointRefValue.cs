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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PointRefValue : Value
    {
        public PointRefValue(Value leftOperand, Value rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.PointRefValue;

        /// <inheritdoc/>
        public override bool IsPointRefValue => true;

        /// <inheritdoc/>
        public override PointRefValue AsPointRefValue => this;

        public Value LeftOperand { get; private set; }
        public Value RightOperand { get; private set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("F0B17590-2ECD-40EA-BE66-4A6CD180DC6A");
        }

        /// <inheritdoc/>
        public override void SetValue(IMonitorLogger logger, Value value)
        {
            if(RightOperand.IsStrongIdentifierValue)
            {
                LeftOperand.SetMemberValue(logger, RightOperand.AsStrongIdentifierValue, value);
                return;
            }

            throw new NotImplementedException("CDE2D17B-296F-40D0-B154-EE0E48A06DCC");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if (LeftOperand != null)
            {
                result ^= LongHashCodeWeights.BaseParamWeight ^ LeftOperand.GetLongHashCode(options);
            }

            if (RightOperand != null)
            {
                result ^= LongHashCodeWeights.BaseParamWeight ^ RightOperand.GetLongHashCode(options);
            }

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new PointRefValue(LeftOperand, RightOperand);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            LeftOperand?.DiscoverAllAnnotations(result);
            RightOperand?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            
            sb.PrintShortObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintShortObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            
            sb.PrintBriefObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintBriefObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return $"{LeftOperand.ToHumanizedString(options)}.{RightOperand.ToHumanizedString(options)}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }
    }
}
