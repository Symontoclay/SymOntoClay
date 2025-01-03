namespace SymOntoClay.Monitor.Common.Data
{
    public class StopBuildPlanMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.StopBuildPlan;
    }
}
