using SymOntoClay.Common.Cancellation;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IObjectWithPeriodicMethod
    {
        bool PeriodicHandler(ICancellationContext cancellationContext);
    }
}
