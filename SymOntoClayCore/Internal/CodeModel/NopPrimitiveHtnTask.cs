/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Text;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class NopPrimitiveHtnTask : BasePrimitiveHtnTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.NopPrimitiveTask;

        /// <inheritdoc/>
        public override bool IsNopPrimitiveHtnTask => true;

        /// <inheritdoc/>
        public override NopPrimitiveHtnTask AsNopPrimitiveHtnTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Nop;

        /// <inheritdoc/>
        public override KindOfPrimitiveTask KindOfPrimitiveTask => KindOfPrimitiveTask.Nop;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override BaseHtnTask CloneBaseTask(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public NopPrimitiveHtnTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public NopPrimitiveHtnTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (NopPrimitiveHtnTask)context[this];
            }

            var result = new NopPrimitiveHtnTask();
            context[this] = result;



            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("1D118861-698C-4842-9780-4719868E5D0C");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }

        private string NToHumanizedString()
        {
            throw new NotImplementedException("2D7381AD-E67B-4D0D-A03D-F9C390548B8E");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            //if (!Cases.IsNullOrEmpty())
            //{
            //    foreach (var item in Cases)
            //    {
            //        result ^= item.GetLongHashCode(options);
            //    }
            //}

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            //Operator.DiscoverAllAnnotations(result);

            //if (!Cases.IsNullOrEmpty())
            //{
            //    foreach (var item in Cases)
            //    {
            //        item.DiscoverAllAnnotations(result);
            //    }
            //}
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintObjProp(n, nameof(Operator), Operator);

            //sb.PrintObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintShortObjProp(n, nameof(Operator), Operator);

            //sb.PrintShortObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintBriefObjProp(n, nameof(Operator), Operator);

            //sb.PrintBriefObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
