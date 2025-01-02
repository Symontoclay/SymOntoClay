using SymOntoClay.ActiveObject.Threads;
using System;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksExecutorComponent: BaseComponent, ITasksExecutorComponent
    {
        public TasksExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
            _activeObject = new AsyncActivePeriodicObject(_context.ActiveObjectContext, _context.CodeExecutionThreadPool, _context.Logger);
            _activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly IEngineContext _context;
        private readonly IActivePeriodicObject _activeObject;
        private TasksPlanner _tasksPlanner;
        private TasksPlanRunner _tasksPlanRunner;

        public void BeginStarting()
        {
            _activeObject.Start();
        }

        public void Init()
        {
#if DEBUG
            Info("BA2BD8D5-63F4-4B71-A6BE-520F8EB9801A", "Begin");
#endif

            //_tasksPlanner = new TasksPlanner(_context);

            //            if(!_tasksPlanner.HasRootTasks)
            //            {
            //#if DEBUG
            //                Info("723CD4E5-CCF4-4C1E-AF2D-DCDA776CA721", "!_tasksPlanner.HasRootTasks");
            //#endif

            //                return;
            //            }

            //_tasksPlanRunner = new TasksPlanRunner(_context, new AsyncActivePeriodicObject(_context.ActiveObjectContext, _context.CodeExecutionThreadPool, _context.Logger));

            //throw new NotImplementedException("272E9EF0-D0BA-4781-AFA7-763783A0F2DF");
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            Info("EB501AF1-9B30-4D5A-ACD1-C013DF7769B8", "Begin !!!!!");
#endif

            return false;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();
            _tasksPlanner?.Dispose();
            _tasksPlanRunner?.Dispose();

            base.OnDisposed();
        }
    }
}
