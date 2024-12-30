using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanRunner: BaseComponent
    {
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

        private TasksPlanFrame _tasksPlanFrame;

        public void Run(TasksPlan plan)
        {
#if DEBUG
            Info("2AC6C566-EA83-476B-92E6-8F955F0B0935", $"plan = {plan.ToDbgString()}");
#endif

            _tasksPlanFrame = ConvertTasksPlanToFrame(plan);

#if DEBUG
            Info("4C249182-C6F2-45B9-9ECC-1C883681ED67", $"_tasksPlanFrame = {_tasksPlanFrame.ToDbgString()}");
#endif

            _activeObject.Start()?.Wait();

            //throw new NotImplementedException("03EEE01B-1763-49D4-AC9B-2A071C887248");
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            Info("BB2C24F5-B3FA-4394-BC6F-7BD5EF4010B7", "Begin");
#endif

            try
            {
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
                    return false;
                }

                var item = tasksPlanFrameItems[currentPosition];

#if DEBUG
                Info("49A45A6E-0F52-4FD3-9B12-376BCC93D70A", $"item = {item}");
#endif

                var executedTask = item.ExecutedTask;

#if DEBUG
                Info("8862EF80-9923-43C9-BBBF-B5E61EA42F98", $"executedTask = {executedTask}");
#endif

                var processInitialInfo = new ProcessInitialInfo();
                processInitialInfo.CompiledFunctionBody = executedTask.Operator.CompiledFunctionBody;
                processInitialInfo.LocalContext = _mainEntity.LocalCodeExecutionContext;
                processInitialInfo.Metadata = executedTask;
                processInitialInfo.Instance = _mainEntity;
                processInitialInfo.ExecutionCoordinator = _mainEntity.ExecutionCoordinator;

                var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);

                task.Wait();

                tasksPlanFrame.CurrentPosition++;

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
                var frameItem = new TasksPlanFrameItem 
                { 
                    ExecutedTask = item.ExecutedTask,
                    ParentTasks = item.ParentTasks.ToList()
                };

                frameItems[n] = frameItem;
                n++;
            }

            var frame = new TasksPlanFrame();
            frame.Items = frameItems;

            return frame;
        }
    }
}
