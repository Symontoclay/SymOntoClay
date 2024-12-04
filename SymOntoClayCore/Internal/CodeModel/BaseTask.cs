using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseTask : CodeItem
    {
        /// <inheritdoc/>
        public override bool IsBaseTask => true;

        /// <inheritdoc/>
        public override BaseTask AsBaseTask => this;

        public abstract BaseTask CloneBaseTask(Dictionary<object, object> context);
    }
}
