namespace SymOntoClay.Monitor.Common.Data
{
    public class BecomeInvisibleMessage: BaseVisionFrameEventMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.BecomeInvisible;
    }
}
