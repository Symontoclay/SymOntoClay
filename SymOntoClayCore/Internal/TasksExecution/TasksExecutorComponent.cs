using System;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksExecutorComponent: BaseComponent, ITasksExecutorComponent
    {
        public TasksExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        public void Init()
        {
            _context.InstancesStorage.OnIdle += InstancesStorage_OnIdle;
        }

        private void InstancesStorage_OnIdle()
        {
#if DEBUG
            Info("BA2BD8D5-63F4-4B71-A6BE-520F8EB9801A", "Begin");
#endif
            //throw new NotImplementedException("272E9EF0-D0BA-4781-AFA7-763783A0F2DF");
        }

        private readonly IEngineContext _context;
    }
}
