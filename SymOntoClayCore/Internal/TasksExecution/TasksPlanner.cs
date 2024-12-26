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

                var currentBuiltPlanItem = buildPlanIterationContext.TasksToProcess[buildPlanIterationContext.ProcessedIndex];

#if DEBUG
                Info("25DD52E6-AD5B-4564-B521-2AD710FCA605", $"currentBuiltPlanItem = {currentBuiltPlanItem}");
#endif

                var kindOfCurrentTask = currentBuiltPlanItem.ProcessedTask.KindOfTask;

#if DEBUG
                Info("2B3D4EAE-A6F3-4340-AEBE-A7668EFC0BB0", $"kindOfCurrentTask = {kindOfCurrentTask}");
#endif

                switch(kindOfCurrentTask)
                {
                    case KindOfTask.Primitive:
                        ProcessPrimitiveTask(currentBuiltPlanItem, tasksPlannerGlobalContext, buildPlanIterationContext);
                        break;

                    case KindOfTask.Compound:
                        ProcessBaseCompoundTask(currentBuiltPlanItem, tasksPlannerGlobalContext, buildPlanIterationContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfCurrentTask), kindOfCurrentTask, null);
                }

                throw new NotImplementedException("2CDDF950-725E-45EC-8D3B-5BD2684F77FD");
            }
        }

        private void ProcessPrimitiveTask(BuiltPlanItem builtPlanItem, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            Info("2707CB78-462A-4BB8-A076-5527C51789DB", "Begin");
#endif

            var processedTask = builtPlanItem.ProcessedTask.AsPrimitiveTask;

#if DEBUG
            Info("5FED19BB-FF10-4804-8FBC-79C0FA1028E4", $"processedTask = {processedTask}");
#endif

            throw new NotImplementedException("774AF910-A2C3-4175-8A84-3A09BFBA87E9");
        }

        private void ProcessBaseCompoundTask(BuiltPlanItem builtPlanItem, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            Info("DF3B0700-5B7E-4101-BB8B-FF159ADF9080", "Begin");
#endif

            var processedTask = builtPlanItem.ProcessedTask.AsBaseCompoundTask;

#if DEBUG
            Info("35B5E17A-C30E-4EF7-91F6-66D1F5E9950A", $"processedTask = {processedTask}");
#endif

            foreach(var taskCase in processedTask.Cases)
            {
#if DEBUG
                Info("0BEBD584-A2F2-496E-800B-E04E6F5F7CED", $"taskCase = {taskCase}");
#endif

                if(!CheckTaskCase(taskCase))
                {
                    continue;
                }

                ProcessTaskCase(taskCase, processedTask, tasksPlannerGlobalContext, buildPlanIterationContext);
            }

            throw new NotImplementedException("20A515FC-9D9F-4185-B14E-12C80C5CFCDD");
        }

        private bool CheckTaskCase(CompoundTaskCase taskCase)
        {
            //It will be implemented when case will have conditions.
            return true;
        }

        private void ProcessTaskCase(CompoundTaskCase taskCase, BaseCompoundTask processedTask, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            Info("90913386-6F54-47D4-B1D6-EC49F29604FC", "Begin");
#endif

            var items = taskCase.Items;

            if (items.IsNullOrEmpty())
            {
                return;
            }

            var clonnedBuildPlanIterationContext = buildPlanIterationContext.Clone();

#if DEBUG
            Info("FB034078-4FD7-4A5E-9BF4-37EB9C32E75D", $"clonnedBuildPlanIterationContext = {clonnedBuildPlanIterationContext}");
#endif

            var baseTasksList = new List<BaseTask>();

            foreach (var item in items)
            {
#if DEBUG
                Info("D99601A0-5F07-417D-921A-0B77E84956AB", $"item = {item}");
#endif

                var task = _tasksStorage.GetBaseTaskByName(Logger, item.Name);

#if DEBUG
                Info("2A728ED1-B0C8-4FC5-AB66-D978F97A91E3", $"task = {task}");
#endif

                if(task == null)
                {
                    return;
                }

                baseTasksList.Add(task);
            }

            ReplaceBuiltPlanItems(baseTasksList, clonnedBuildPlanIterationContext);

            ProcessIteration(tasksPlannerGlobalContext, clonnedBuildPlanIterationContext);

            throw new NotImplementedException("40A79CD7-9DCB-4B93-BDB2-A6F328E79CA4");
        }

        private void ReplaceBuiltPlanItems(List<BaseTask> baseTasks, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            Info("174DE9EE-0069-4F7F-991E-EF8AB0417996", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

            throw new NotImplementedException("9C08E57B-F9A1-4041-9C6A-16BF2BCA188C");
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
