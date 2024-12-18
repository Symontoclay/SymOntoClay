namespace SymOntoClay.Core.Internal.Storage.TasksStoraging
{
    public class TasksStorage: BaseSpecificStorage, ITasksStorage
    {
        public TasksStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }
    }
}
