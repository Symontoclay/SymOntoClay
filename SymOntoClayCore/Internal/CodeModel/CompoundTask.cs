using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundTask: BaseCompoundTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundTask;

        /// <inheritdoc/>
        public override bool IsCompoundTask => true;

        /// <inheritdoc/>
        public override CompoundTask AsCompoundTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Compound;

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
        public CompoundTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundTask)context[this];
            }

            var result = new CompoundTask();
            context[this] = result;

            result.AppendBaseCompoundTask(this, context);

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
    }
}
