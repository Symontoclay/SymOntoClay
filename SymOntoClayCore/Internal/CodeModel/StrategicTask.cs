using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StrategicTask : BaseCompoundTask
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.StrategicTask;

        /// <inheritdoc/>
        public override bool IsStrategicTask => true;

        /// <inheritdoc/>
        public override StrategicTask AsStrategicTask => this;

        /// <inheritdoc/>
        public override KindOfTask KindOfTask => KindOfTask.Strategic;

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
        public StrategicTask Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public StrategicTask Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (StrategicTask)context[this];
            }

            var result = new StrategicTask();
            context[this] = result;

            result.AppendBaseCompoundTask(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("7543887B-62D4-41E3-99F4-E4C96CA71195");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("B8E14BB8-5310-468D-812E-F535B42E960F");
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("DED5BAAB-E9AB-4636-AA09-A15128E3809B");
        }
    }
}
