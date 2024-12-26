using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanner: BaseComponent
    {
        public TasksPlanner(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
            _mainEntity = _context.InstancesStorage.MainEntity;
            _tasksStorage = _context.Storage.GlobalStorage.TasksStorage;
        }

        private readonly IEngineContext _context;
        private readonly AppInstance _mainEntity;
        private readonly ITasksStorage _tasksStorage;

        public TasksPlan BuildPlan()
        {
            var rootTasks = GetRootTasks();

#if DEBUG
            Info("54AF89ED-3EBB-4A31-86A9-577EE1AF5190", $"rootTasks = {rootTasks.WriteListToString()}");
#endif

            if(!rootTasks.Any())
            {
                return TasksPlan.EmptyPlan;
            }

            //TODO: make processing multiple root taks for working with voice commands.
            var rootTask = rootTasks.Single();

            var tasksPlannerGlobalContext = new TasksPlannerGlobalContext();

            var buildPlanIterationContext = new BuildPlanIterationContext();
            buildPlanIterationContext.TasksToProcess.Add(new BuiltPlanItem
            {
                ProcessedTask = rootTask
            });

#if DEBUG
            Info("1E36D336-39B2-4548-BFCD-12E072D51755", $"buildPlanIterationContext = {buildPlanIterationContext}");
            Info("D531DC15-FE0B-4134-AA25-953997E60221", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

            ProcessIteration(tasksPlannerGlobalContext, buildPlanIterationContext);

            throw new NotImplementedException("FF8CD857-079E-49A1-8C06-32D475C38D56");
        }

        private void ProcessIteration(TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            Info("FDD9D703-6231-450A-9B55-AA2734952558", "Begin");
#endif

#if DEBUG
            Info("8BA1B85B-A1DF-4ABB-9CE4-925E8190303A", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

            while(true)
            {
                buildPlanIterationContext.ProcessedIndex++;

#if DEBUG
                Info("ABDE6F0C-CA9B-49DB-9377-DB3092F19827", $"buildPlanIterationContext.ProcessedIndex (after) = {buildPlanIterationContext.ProcessedIndex}");
#endif

                throw new NotImplementedException("2CDDF950-725E-45EC-8D3B-5BD2684F77FD");
            }
        }

        private List<BaseCompoundTask> GetRootTasks()
        {
#if DEBUG
            //Info("47323362-D3B3-47FD-B346-D746771DB8C7", $"_mainEntity.Name = {_mainEntity.Name}");
            //Info("600EDBC3-9F4F-43AE-B900-C029F4BB1AEC", $"_mainEntity.GetType().Name = {_mainEntity.GetType().Name}");
            //Info("E40E6248-2A1C-4CF7-95AB-BBC82924C46E", $"_mainEntity.RootTasks = {_mainEntity.RootTasks.WriteListToString()}");
#endif

            var result = new List<BaseCompoundTask>();

            var mainEntityRootTasksNames = _mainEntity.RootTasks;

            if(!mainEntityRootTasksNames.IsNullOrEmpty())
            {
                foreach(var taskName in mainEntityRootTasksNames)
                {
                    var task = _tasksStorage.GetBaseCompoundTaskByName(Logger, taskName);

#if DEBUG
                    //Info("627C7805-7915-4787-BB76-E7D2B2B7EE56", $"task = {task}");
#endif

                    if(task == null)
                    {
                        continue;
                    }

                    result.Add(task);
                }
            }

            return result;
        }
    }
}
