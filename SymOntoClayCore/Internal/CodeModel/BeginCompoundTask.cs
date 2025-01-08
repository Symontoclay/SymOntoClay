using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class BeginCompoundTask: BasePrimitiveTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.BeginCompoundTask;

        /// <inheritdoc/>
        public override bool IsBeginCompoundTask => true;

        /// <inheritdoc/>
        public override BeginCompoundTask AsBeginCompoundTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.BeginCompound;

        /// <inheritdoc/>
        public override KindOfPrimitiveTask KindOfPrimitiveTask => KindOfPrimitiveTask.BeginCompound;

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
        public BeginCompoundTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public BeginCompoundTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (BeginCompoundTask)context[this];
            }

            var result = new BeginCompoundTask();
            context[this] = result;

            

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("DDA3C2FA-CB14-41FB-B057-49F163989147");
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
            throw new NotImplementedException("37EBF6B5-E5FD-4A8F-AB41-A72CDBE25369");
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
