using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RootTask : BaseCompoundTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.RootTask;

        /// <inheritdoc/>
        public override bool IsRootTask => true;

        /// <inheritdoc/>
        public override RootTask AsRootTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Root;

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

        /// <inheritdoc/>
        public override BaseCompoundTask CloneBaseCompoundTask(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public RootTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public RootTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RootTask)context[this];
            }

            var result = new RootTask();
            context[this] = result;

            result.AppendBaseCompoundTask(this, context);

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
            return $"root task: {Name?.ToSystemString()}";
        }
    }
}
