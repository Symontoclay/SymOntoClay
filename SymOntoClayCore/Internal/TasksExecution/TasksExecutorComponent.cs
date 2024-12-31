namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksExecutorComponent: BaseComponent, ITasksExecutorComponent
    {
        public TasksExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;
    }
}
