using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Compiling;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksExecutorComponent: BaseContextComponent, ITasksExecutorComponent
    {
        private enum ExecutionState
        {
            Init,
            WaitingForPlanBuilding,
            WaitingForStartPlanExecution,
            WaitingForFinishPlanExecution
        }

        public TasksExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _compiler = _context.Compiler;
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            _activeObject = new AsyncActivePeriodicObject(_context.ActiveObjectContext, _context.CodeExecutionThreadPool, _context.Logger);
            _activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly IEngineContext _context;
        private IActivePeriodicObject _activeObject;
        private volatile TasksPlanner _tasksPlanner;
        private ICompiler _compiler;
        private volatile ExecutionState _executionState = ExecutionState.Init;
        private volatile TasksPlan _plan;
        private volatile IThreadExecutor _threadExecutor;

        public void BeginStarting()
        {
            _activeObject.Start();
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            //Info("EB501AF1-9B30-4D5A-ACD1-C013DF7769B8", $"_executionState = {_executionState}");
#endif

//            if (_executionState == ExecutionState.Init)
//            {
//                _tasksPlanner = new TasksPlanner(_context);

//                if (!_tasksPlanner.HasRootTasks)
//                {
//#if DEBUG
//                    //Info("723CD4E5-CCF4-4C1E-AF2D-DCDA776CA721", "!_tasksPlanner.HasRootTasks");
//#endif

//                    return false;
//                }

//                _executionState = ExecutionState.WaitingForPlanBuilding;
//            }

//            if (_executionState == ExecutionState.WaitingForPlanBuilding)
//            {
//                Logger.StartBuildPlan("584A66AE-6592-4476-AA54-C283C9A65DBE");

//                _plan = _tasksPlanner.BuildPlan();

//                Logger.StopBuildPlan("CEB8C398-CFA8-4869-A6BC-5BFB32F89CBF");

//                _executionState = ExecutionState.WaitingForStartPlanExecution;
//            }

//            if (_executionState == ExecutionState.WaitingForStartPlanExecution)
//            {
//                var compiledFunctionBody = _compiler.Compile(_plan);

//                var mainEntity = _context.InstancesStorage.MainEntity;

//                var processInitialInfo = new ProcessInitialInfo();
//                processInitialInfo.CompiledFunctionBody = compiledFunctionBody;
//                processInitialInfo.LocalContext = mainEntity.LocalCodeExecutionContext;
//                processInitialInfo.Metadata = mainEntity.CodeItem;
//                processInitialInfo.Instance = mainEntity;
//                processInitialInfo.ExecutionCoordinator = mainEntity.ExecutionCoordinator;

//                _threadExecutor = _context.CodeExecutor.ExecuteAsync(_context.Logger, processInitialInfo);

//                _executionState = ExecutionState.WaitingForFinishPlanExecution;
//            }

//            if(_executionState == ExecutionState.WaitingForFinishPlanExecution)
//            {
//                var runningStatus = _threadExecutor.RunningStatus;

//#if DEBUG
//                //Info("8729B582-40C3-431E-9C27-C8413C1F94E0", $"runningStatus = {runningStatus}");
//#endif

//                if(runningStatus == Threading.ThreadTaskStatus.RanToCompletion ||
//                    runningStatus == Threading.ThreadTaskStatus.Canceled ||
//                    runningStatus == Threading.ThreadTaskStatus.Faulted)
//                {
//                    _threadExecutor = null;
//                    _executionState = ExecutionState.WaitingForPlanBuilding;
//                }
//            }

//            return true;

            return false;//tmp
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();
            _tasksPlanner?.Dispose();
            
            base.OnDisposed();
        }
    }
}
