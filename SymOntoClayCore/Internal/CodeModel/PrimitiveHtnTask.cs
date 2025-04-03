using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimitiveHtnTask: BasePrimitiveHtnTask
    {
        public PrimitiveHtnTaskOperator Operator { get; set; }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.PrimitiveTask;

        /// <inheritdoc/>
        public override bool IsPrimitiveTask => true;

        /// <inheritdoc/>
        public override PrimitiveHtnTask AsPrimitiveTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Primitive;

        /// <inheritdoc/>
        public override KindOfPrimitiveTask KindOfPrimitiveTask => KindOfPrimitiveTask.Primitive;

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
        public PrimitiveHtnTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimitiveHtnTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimitiveHtnTask)context[this];
            }

            var result = new PrimitiveHtnTask();
            context[this] = result;

            result.Operator = Operator?.Clone(context);

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
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
            return $"primitive task: {Name.NameValue}";
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
