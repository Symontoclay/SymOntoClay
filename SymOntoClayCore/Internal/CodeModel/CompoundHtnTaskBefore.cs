using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundHtnTaskBefore : CompoundHtnTaskItemsSection
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundHtnTaskBefore;

        /// <inheritdoc/>
        public override bool IsCompoundHtnTaskBefore => true;

        /// <inheritdoc/>
        public override CompoundHtnTaskBefore AsCompoundHtnTaskBefore => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompoundHtnTaskBefore Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundHtnTaskBefore Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundHtnTaskBefore)context[this];
            }

            var result = new CompoundHtnTaskBefore();
            context[this] = result;

            FillUpCompoundHtnTaskItemsSection(result, context);

            return result;
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

            sb.AppendLine("before");

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
            throw new NotImplementedException("41E884DA-017F-4701-BDB4-1A3C61D6FF77");
        }
    }
}
