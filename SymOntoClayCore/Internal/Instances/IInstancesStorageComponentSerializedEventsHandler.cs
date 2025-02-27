namespace SymOntoClay.Core.Internal.Instances
{
    public interface IInstancesStorageComponentSerializedEventsHandler
    {
        void NDispatchOnIdle();
        void NOnFinishProcessWithDevicesHandler(IProcessInfo sender);
    }
}
