using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanRunner: BaseComponent
    {
        private enum ExecutionState
        {
            Init,
            PrepareToItemExecution,
            WaitingForFinishItemExecution,
            FinishItemExecution
        }

        public TasksPlanRunner(IEngineContext context, IActivePeriodicObject activeObject)
            : base(context.Logger)
        {
            _context = context;
            _mainEntity = _context.InstancesStorage.MainEntity;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly IEngineContext _context;
        private readonly AppInstance _mainEntity;
        private readonly IActivePeriodicObject _activeObject;
        private volatile ExecutionState _executionState = ExecutionState.Init;

        private volatile TasksPlanFrame _tasksPlanFrame;
        private volatile IThreadTask _task;

        public void Run(TasksPlan plan)
        {
#if DEBUG
            //Info("2AC6C566-EA83-476B-92E6-8F955F0B0935", $"plan = {plan.ToDbgString()}");
#endif

            _tasksPlanFrame = ConvertTasksPlanToFrame(plan);
            
#if DEBUG
            Info("4C249182-C6F2-45B9-9ECC-1C883681ED67", $"_tasksPlanFrame = {_tasksPlanFrame.ToDbgString()}");
#endif

            _activeObject.Start()?.Wait();

#if DEBUG
            Info("1D687F2D-4708-4753-B147-641B37AAF99B", "#####################################");
#endif

            //throw new NotImplementedException("03EEE01B-1763-49D4-AC9B-2A071C887248");
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            Info("BB2C24F5-B3FA-4394-BC6F-7BD5EF4010B7", $"_executionState = {_executionState}");
#endif

            try
            {
                if(_executionState == ExecutionState.Init)
                {
                    _executionState = ExecutionState.PrepareToItemExecution;
                }

                var tasksPlanFrame = _tasksPlanFrame;

#if DEBUG
                Info("B3F39DA7-3638-4DC9-80EF-0C8C7C33CA8D", $"tasksPlanFrame = {tasksPlanFrame.ToDbgString()}");
#endif

                var currentPosition = tasksPlanFrame.CurrentPosition;

#if DEBUG
                Info("4DB03AB0-16D3-4A03-AA88-A9B2644E60B6", $"currentPosition = {currentPosition}");
#endif

                var tasksPlanFrameItems = tasksPlanFrame.Items;

                if(currentPosition >= tasksPlanFrameItems.Count)
                {
                    Logger.LeaveTasksExecutor("E6F885F9-5D83-4B4C-BAE2-2ED2D7B9EFAD");

                    return false;
                }

                var item = tasksPlanFrameItems[currentPosition];

                Logger.PlanFrame("850FC994-E31A-4736-8519-DDDED1F13858", tasksPlanFrame.ToDbgString());

#if DEBUG
                //Info("49A45A6E-0F52-4FD3-9B12-376BCC93D70A", $"item = {item}");
#endif

                var executedTask = item.ExecutedTask;

#if DEBUG
                //Info("8862EF80-9923-43C9-BBBF-B5E61EA42F98", $"executedTask = {executedTask}");
#endif

                var primitiveTaskId = 0ul;

                if(_executionState == ExecutionState.PrepareToItemExecution)
                {
                    primitiveTaskId = Logger.StartPrimitiveTask("CCAF4110-C1C7-4999-9ECB-351F8DBD6DB6");

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = executedTask.Operator.CompiledFunctionBody;
                    processInitialInfo.LocalContext = _mainEntity.LocalCodeExecutionContext;
                    processInitialInfo.Metadata = executedTask;
                    processInitialInfo.Instance = _mainEntity;
                    processInitialInfo.ExecutionCoordinator = _mainEntity.ExecutionCoordinator;

                    _task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);

                    _executionState = ExecutionState.WaitingForFinishItemExecution;
                }

                if(_executionState == ExecutionState.WaitingForFinishItemExecution)
                {
                    _task.Wait();

                    Logger.StopPrimitiveTask("E9AA3F24-8A81-4F4C-81A6-4FBC3C2CC286", primitiveTaskId);

                    _executionState = ExecutionState.FinishItemExecution;
                }

                if(_executionState == ExecutionState.FinishItemExecution)
                {
                    tasksPlanFrame.CurrentPosition++;

                    _executionState = ExecutionState.PrepareToItemExecution;
                }

                return true;
            }
            catch (Exception e)
            {
                Error("C4FD1E93-98B8-46C1-8FB5-89B7FB675709", e);

                throw;
            }
        }

        private TasksPlanFrame ConvertTasksPlanToFrame(TasksPlan plan)
        {
            var frameItems = new Dictionary<int, TasksPlanFrameItem>();

            var n = 0;

            foreach(var item in plan.Items)
            {
                throw new NotImplementedException("34F74B8F-4616-4DBD-B633-0FFBB3098F01");

                var frameItem = new TasksPlanFrameItem 
                { 
                    //ExecutedTask = item.ExecutedTask,
                    ParentTasks = item.ParentTasks.ToList()
                };

                frameItems[n] = frameItem;
                n++;
            }

            var frame = new TasksPlanFrame();
            frame.Items = frameItems;

            return frame;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();

            base.OnDisposed();
        }
    }
}
