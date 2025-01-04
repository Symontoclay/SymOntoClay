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
            throw new NotImplementedException("77B5C810-E90E-41A9-A7C0-FB03C9B01DCC");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("18AAAF75-827C-47EC-868B-DD9DD5C5D085");
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("600EE883-BB5B-4A8E-84BB-FAAF2424C549");
        }
    }
}
