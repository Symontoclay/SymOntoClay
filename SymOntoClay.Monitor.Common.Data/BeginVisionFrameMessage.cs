namespace SymOntoClay.Monitor.Common.Data
{
    public class BeginVisionFrameMessage: BaseVisionFrameMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.BeginVisionFrame;
    }
}
