namespace SymOntoClay.Monitor.Common.Data
{
    public class StopPrimitiveTaskMessage : BasePrimitiveTaskMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.StopPrimitiveTask;
    }
}
