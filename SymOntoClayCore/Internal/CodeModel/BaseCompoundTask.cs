using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseCompoundTask: BaseTask
    {
        /// <inheritdoc/>
        public override bool IsBaseCompoundTask => true;

        /// <inheritdoc/>
        public override BaseCompoundTask AsBaseCompoundTask => this;

        public abstract BaseCompoundTask CloneBaseCompoundTask(Dictionary<object, object> context);

        protected void AppendBaseCompoundTask(BaseCompoundTask source, Dictionary<object, object> context)
        {
            AppendCodeItem(source, context);
        }
    }
}
