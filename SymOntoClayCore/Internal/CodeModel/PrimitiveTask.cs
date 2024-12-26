using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using SymOntoClay.Common.DebugHelpers;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimitiveTask : BaseTask
    {
        public PrimitiveTaskOperator Operator { get; set; }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.PrimitiveTask;

        /// <inheritdoc/>
        public override bool IsPrimitiveTask => true;

        /// <inheritdoc/>
        public override PrimitiveTask AsPrimitiveTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Primitive;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override BaseTask CloneBaseTask(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PrimitiveTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimitiveTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimitiveTask)context[this];
            }

            var result = new PrimitiveTask();
            context[this] = result;

            result.Operator = Operator.Clone(context);

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("818F06F8-E3EA-4DC7-97E0-D9D916BDF85D");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("A1A25B04-474D-450B-A8F1-E362C6FDC91D");
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("8A82FBD7-5B9B-444F-A476-D5DD5A1F91C2");
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

            Operator.DiscoverAllAnnotations(result);

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

            sb.PrintObjProp(n, nameof(Operator), Operator);

            //sb.PrintObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Operator), Operator);

            //sb.PrintShortObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Operator), Operator);

            //sb.PrintBriefObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
