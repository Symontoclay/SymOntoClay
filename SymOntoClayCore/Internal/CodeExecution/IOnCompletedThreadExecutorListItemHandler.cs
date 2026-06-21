namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IOnCompletedThreadExecutorListItemHandler
    {
        void Invoke(ThreadExecutorListItem sender);
    }
}
