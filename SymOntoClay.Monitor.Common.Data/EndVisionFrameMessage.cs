namespace SymOntoClay.Monitor.Common.Data
{
    public class EndVisionFrameMessage: BaseVisionFrameMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.EndVisionFrame;
    }
}
