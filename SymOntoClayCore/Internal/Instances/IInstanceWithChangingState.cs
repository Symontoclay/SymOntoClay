namespace SymOntoClay.Core.Internal.Instances
{
    public interface IInstanceWithChangingState
    {
        InstanceState InstanceState { get; set; }
    }
}
