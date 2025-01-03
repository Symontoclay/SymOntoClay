namespace SymOntoClay.Monitor.Common.Data
{
    public class StartBuildPlanMessage : BaseMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.StartBuildPlan;
    }
}
