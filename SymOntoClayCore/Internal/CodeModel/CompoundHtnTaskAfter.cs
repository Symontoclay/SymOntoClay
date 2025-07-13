using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundHtnTaskAfter : CompoundHtnTaskItemsSection
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundHtnTaskAfter;

        /// <inheritdoc/>
        public override bool IsCompoundHtnTaskAfter => true;

        /// <inheritdoc/>
        public override CompoundHtnTaskAfter AsCompoundHtnTaskAfter => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompoundHtnTaskAfter Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundHtnTaskAfter Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundHtnTaskAfter)context[this];
            }

            var result = new CompoundHtnTaskAfter();
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

            sb.AppendLine("after");

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
            throw new NotImplementedException("E7AB0403-1BC2-4FEC-B172-53A86717732B");
        }
    }
}
