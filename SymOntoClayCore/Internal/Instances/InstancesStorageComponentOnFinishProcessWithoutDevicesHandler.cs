using SymOntoClay.Core.EventsInterfaces;

namespace SymOntoClay.Core.Internal.Instances
{
    public class InstancesStorageComponentOnFinishProcessWithoutDevicesHandler: IOnFinishProcessInfoHandler
    {
        public InstancesStorageComponentOnFinishProcessWithoutDevicesHandler(InstancesStorageComponent instancesStorageComponent)
        {
            _instancesStorageComponent = instancesStorageComponent;
        }

        private InstancesStorageComponent _instancesStorageComponent;

        void IOnFinishProcessInfoHandler.Invoke(IProcessInfo sender)
        {
            _instancesStorageComponent.OnFinishProcessWithoutDevicesHandler(sender);
        }
    }
}
