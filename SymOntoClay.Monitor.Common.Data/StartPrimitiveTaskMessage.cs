namespace SymOntoClay.Monitor.Common.Data
{
    public class StartPrimitiveTaskMessage : BasePrimitiveTaskMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.StartPrimitiveTask;
    }
}
