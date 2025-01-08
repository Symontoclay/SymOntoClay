using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System.Text;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class EndCompoundTask : BasePrimitiveTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.EndCompoundTask;

        /// <inheritdoc/>
        public override bool IsEndCompoundTask => true;

        /// <inheritdoc/>
        public override EndCompoundTask AsEndCompoundTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.EndCompound;

        /// <inheritdoc/>
        public override KindOfPrimitiveTask KindOfPrimitiveTask => KindOfPrimitiveTask.EndCompound;

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
        public EndCompoundTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public EndCompoundTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (EndCompoundTask)context[this];
            }

            var result = new EndCompoundTask();
            context[this] = result;



            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("7A51B29B-6348-4E9E-8C68-D5569BE0D91C");
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
            throw new NotImplementedException("DFBB2D6E-2AF0-4FC7-875E-61F12F2E482E");
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
