using SymOntoClay.Common.Cancellation;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IObjectWithOnceRunMethod
    {
        void OnceRunHandler(ICancellationContext cancellationContext);
    }

    public interface IObjectWithOnceRunMethod<TResult>
    {
        TResult OnceRunHandler(ICancellationContext cancellationContext);
    }
}
