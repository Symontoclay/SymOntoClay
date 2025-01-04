using SymOntoClay.ActiveObject.Threads;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksExecutorComponent: BaseComponent, ITasksExecutorComponent
    {
        private enum ExecutionState
        {
            Init,
            WaitingForPlanBuilding,
            WaitingForRunnerCreation,
            WaitingForPlanExecution
        }

        public TasksExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
            _activeObject = new AsyncActivePeriodicObject(_context.ActiveObjectContext, _context.CodeExecutionThreadPool, _context.Logger);
            _activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly IEngineContext _context;
        private readonly IActivePeriodicObject _activeObject;
        private volatile TasksPlanner _tasksPlanner;
        private volatile TasksPlanRunner _tasksPlanRunner;
        private volatile ExecutionState _executionState = ExecutionState.Init;
        private volatile TasksPlan _plan;

        public void BeginStarting()
        {
            _activeObject.Start();
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            Info("EB501AF1-9B30-4D5A-ACD1-C013DF7769B8", $"_executionState = {_executionState}");
#endif

            if(_executionState == ExecutionState.Init)
            {
                _tasksPlanner = new TasksPlanner(_context);

                if (!_tasksPlanner.HasRootTasks)
                {
#if DEBUG
                    Info("723CD4E5-CCF4-4C1E-AF2D-DCDA776CA721", "!_tasksPlanner.HasRootTasks");
#endif

                    return false;
                }

                _executionState = ExecutionState.WaitingForPlanBuilding;
            }

            if(_executionState == ExecutionState.WaitingForPlanBuilding)
            {
                Logger.StartBuildPlan("584A66AE-6592-4476-AA54-C283C9A65DBE");

                _plan = _tasksPlanner.BuildPlan();

                Logger.StopBuildPlan("CEB8C398-CFA8-4869-A6BC-5BFB32F89CBF");

                _executionState = ExecutionState.WaitingForRunnerCreation;
            }

            if(_executionState == ExecutionState.WaitingForRunnerCreation)
            {
                _tasksPlanRunner = new TasksPlanRunner(_context, new AsyncActivePeriodicObject(_context.ActiveObjectContext, _context.CodeExecutionThreadPool, _context.Logger));

                _executionState = ExecutionState.WaitingForPlanExecution;
            }

            if(_executionState == ExecutionState.WaitingForPlanExecution)
            {
                _tasksPlanRunner.Run(_plan);
                
                _executionState = ExecutionState.WaitingForPlanBuilding;
            }

            return true;
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
