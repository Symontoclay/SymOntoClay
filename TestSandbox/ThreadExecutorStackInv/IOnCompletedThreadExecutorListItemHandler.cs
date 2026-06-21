namespace TestSandbox.ThreadExecutorStackInv
{
    public interface IOnCompletedThreadExecutorListItemHandler
    {
        void Invoke(ThreadExecutorListItem sender);
    }
}
