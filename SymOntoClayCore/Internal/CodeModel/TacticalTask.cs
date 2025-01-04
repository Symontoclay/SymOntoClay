using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class TacticalTask : BaseCompoundTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.TacticalTask;

        /// <inheritdoc/>
        public override bool IsTacticalTask => true;

        /// <inheritdoc/>
        public override TacticalTask AsTacticalTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Tactical;

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
        public TacticalTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public TacticalTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (TacticalTask)context[this];
            }

            var result = new TacticalTask();
            context[this] = result;

            result.AppendBaseCompoundTask(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("39C1D57F-6948-4174-9DFF-7FEE623C709D");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("F197755C-EF76-402A-833A-61174F674AAF");
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("EE085A6F-3AA1-43B3-8B58-52CF3915E1A6");
        }
    }
}
