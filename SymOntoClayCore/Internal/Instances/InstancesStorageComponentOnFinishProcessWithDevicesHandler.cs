using SymOntoClay.Core.EventsInterfaces;

namespace SymOntoClay.Core.Internal.Instances
{
    public class InstancesStorageComponentOnFinishProcessWithDevicesHandler : IOnFinishProcessInfoHandler
    {
        public InstancesStorageComponentOnFinishProcessWithDevicesHandler(InstancesStorageComponent instancesStorageComponent)
        {
            _instancesStorageComponent = instancesStorageComponent;
        }

        private InstancesStorageComponent _instancesStorageComponent;

        void IOnFinishProcessInfoHandler.Invoke(IProcessInfo sender)
        {
            _instancesStorageComponent.OnFinishProcessWithDevicesHandler(sender);
        }
    }
}
