namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BasePrimitiveTask : BaseTask
    {
        /// <inheritdoc/>
        public override bool IsBasePrimitiveTask => true;

        /// <inheritdoc/>
        public override BasePrimitiveTask AsBasePrimitiveTask => this;
    }
}
