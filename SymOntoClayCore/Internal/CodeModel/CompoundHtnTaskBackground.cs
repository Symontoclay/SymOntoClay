using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundHtnTaskBackground : CompoundHtnTaskItemsSection
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundHtnTaskBackground;

        /// <inheritdoc/>
        public override bool IsCompoundHtnTaskBackground => true;

        /// <inheritdoc/>
        public override CompoundHtnTaskBackground AsCompoundHtnTaskBackground => this;

        public TriggerConditionNode Condition { get; set; }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompoundHtnTaskBackground Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundHtnTaskBackground Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundHtnTaskBackground)context[this];
            }

            var result = new CompoundHtnTaskBackground();
            context[this] = result;

            result.Condition = Condition?.Clone(context);

            FillUpCompoundHtnTaskItemsSection(result, context);

            return result;
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode(CheckDirtyOptions options)
        {
            //Condition.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            if (Condition != null)
            {
                Condition.CheckDirty(options);

                result ^= Condition.GetLongHashCode(options);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintObjProp(n, nameof(Condition), Condition);

            sb.PrintBriefObjProp(n, nameof(Condition), Condition);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintShortObjProp(n, nameof(Condition), Condition);

            sb.PrintBriefObjProp(n, nameof(Condition), Condition);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintBriefObjProp(n, nameof(Condition), Condition);

            sb.PrintBriefObjProp(n, nameof(Condition), Condition);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}'{ToHumanizedString()}'";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            sb.Append("background");

            if (Condition != null)
            {
                sb.Append($" {Condition.ToHumanizedString(options)}");
            }

            sb.AppendLine();

            sb.Append(ContentToHumanizedString(options));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("2BC5DCEF-B94B-4F58-B1F8-D21F9B518565");
        }
    }
}
