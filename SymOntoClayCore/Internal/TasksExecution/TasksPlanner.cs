using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool HasRootTasks => GetRootTasks().Count > 0;

        public TasksPlan BuildPlan()
        {
            var rootTasks = GetRootTasks();

#if DEBUG
            //Info("54AF89ED-3EBB-4A31-86A9-577EE1AF5190", $"rootTasks = {rootTasks.WriteListToString()}");
#endif

            if(rootTasks.Count == 0)
            {
                return TasksPlan.EmptyPlan;
            }

            if(rootTasks.Count > 1)
            {
                throw new NotImplementedException("0A44F791-B382-4B29-8F48-B62EB772014E");
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
            //Info("1E36D336-39B2-4548-BFCD-12E072D51755", $"buildPlanIterationContext = {buildPlanIterationContext}");
            //Info("D531DC15-FE0B-4134-AA25-953997E60221", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

            ProcessIteration(tasksPlannerGlobalContext, buildPlanIterationContext);

#if DEBUG
            //Info("75115FF7-6822-4AB7-AE76-633B44127845", $"tasksPlannerGlobalContext = {tasksPlannerGlobalContext}");
#endif

            var completedIterations = tasksPlannerGlobalContext.CompletedIterations;

            if(completedIterations.Count == 0)
            {
                return TasksPlan.EmptyPlan;
            }

            var targetCompletedIteration = GetTargetCompletedIteration(completedIterations);

            return ConvertCompletedIterationToTasksPlan(targetCompletedIteration);
        }

        private TasksPlan ConvertCompletedIterationToTasksPlan(BuildPlanIterationContext completedIteration)
        {
#if DEBUG
            //Info("0ACC770A-B3CD-4182-8E86-4D7CF4F660D5", $"completedIteration = {completedIteration}");
#endif

            var result = new TasksPlan();

            var builtItems = new List<TasksPlanItem>();
            result.Items = builtItems;

            foreach (var builtItem in completedIteration.TasksToProcess)
            {
#if DEBUG
                //Info("EA3F4844-5C18-4F00-8C30-8BDB6BAD6DF2", $"builtItem = {builtItem.ToDbgString()}");
#endif

                var planItem = new TasksPlanItem()
                {
                    ExecutedTask = builtItem.ProcessedTask.AsPrimitiveTask,
                    ParentTasks = builtItem.ParentTasks.Select(p => p.AsBaseCompoundTask).ToList()
                };

#if DEBUG
                //Info("225B84C4-C4A7-4F58-ADE4-0E1932083051", $"planItem = {planItem}");
#endif

                builtItems.Add(planItem);
            }

#if DEBUG
            //Info("B08C997D-DB54-482F-A5F7-90C408031B74", $"result = {result}");
#endif

            return result;
        }

        private BuildPlanIterationContext GetTargetCompletedIteration(List<BuildPlanIterationContext> completedIterations)
        {
            if (completedIterations.Count > 1)
            {
                throw new NotImplementedException("D6E4711B-5922-4BBC-BFA4-EC678A035B06");
            }

            return completedIterations[0];
        }

        private void ProcessIteration(TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("FDD9D703-6231-450A-9B55-AA2734952558", "Begin");
#endif

#if DEBUG
            //Info("8BA1B85B-A1DF-4ABB-9CE4-925E8190303A", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
            //Info("8BA1B85B-A1DF-4ABB-9CE4-925E8190303A", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

            while(true)
            {
                buildPlanIterationContext.ProcessedIndex++;

#if DEBUG
                //Info("ABDE6F0C-CA9B-49DB-9377-DB3092F19827", $"buildPlanIterationContext.ProcessedIndex (after) = {buildPlanIterationContext.ProcessedIndex}");
#endif

                var tasksToProcess = buildPlanIterationContext.TasksToProcess;

                if (buildPlanIterationContext.ProcessedIndex == tasksToProcess.Count)
                {
                    if(tasksToProcess.All(p => p.ProcessedTask.IsPrimitiveTask))
                    {
                        tasksPlannerGlobalContext.CompletedIterations.Add(buildPlanIterationContext);
                    }                   

                    return;
                }

                var currentBuiltPlanItem = tasksToProcess[buildPlanIterationContext.ProcessedIndex];

#if DEBUG
                //Info("25DD52E6-AD5B-4564-B521-2AD710FCA605", $"currentBuiltPlanItem = {currentBuiltPlanItem}");
#endif

                var kindOfCurrentTask = currentBuiltPlanItem.ProcessedTask.KindOfTask;

#if DEBUG
                //Info("2B3D4EAE-A6F3-4340-AEBE-A7668EFC0BB0", $"kindOfCurrentTask = {kindOfCurrentTask}");
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

                //throw new NotImplementedException("2CDDF950-725E-45EC-8D3B-5BD2684F77FD");
            }
        }

        private void ProcessPrimitiveTask(BuiltPlanItem builtPlanItem, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("2707CB78-462A-4BB8-A076-5527C51789DB", "Begin");
#endif

            var processedTask = builtPlanItem.ProcessedTask.AsPrimitiveTask;

#if DEBUG
            //Info("5FED19BB-FF10-4804-8FBC-79C0FA1028E4", $"processedTask = {processedTask}");
#endif

            //throw new NotImplementedException("774AF910-A2C3-4175-8A84-3A09BFBA87E9");
        }

        private void ProcessBaseCompoundTask(BuiltPlanItem builtPlanItem, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("DF3B0700-5B7E-4101-BB8B-FF159ADF9080", "Begin");
#endif

            var processedTask = builtPlanItem.ProcessedTask.AsBaseCompoundTask;

#if DEBUG
            //Info("35B5E17A-C30E-4EF7-91F6-66D1F5E9950A", $"processedTask = {processedTask}");
#endif

            foreach(var taskCase in processedTask.Cases)
            {
#if DEBUG
                //Info("0BEBD584-A2F2-496E-800B-E04E6F5F7CED", $"taskCase = {taskCase}");
#endif

                if(!CheckTaskCase(taskCase))
                {
                    continue;
                }

                ProcessTaskCase(taskCase, tasksPlannerGlobalContext, buildPlanIterationContext);
            }

            //throw new NotImplementedException("20A515FC-9D9F-4185-B14E-12C80C5CFCDD");
        }

        private bool CheckTaskCase(CompoundTaskCase taskCase)
        {
            //It will be implemented when case will have conditions.
            return true;
        }

        private void ProcessTaskCase(CompoundTaskCase taskCase, TasksPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("90913386-6F54-47D4-B1D6-EC49F29604FC", "Begin");
#endif

            var items = taskCase.Items;

            if (items.IsNullOrEmpty())
            {
                return;
            }

            var clonnedBuildPlanIterationContext = buildPlanIterationContext.Clone();

#if DEBUG
            //Info("FB034078-4FD7-4A5E-9BF4-37EB9C32E75D", $"clonnedBuildPlanIterationContext = {clonnedBuildPlanIterationContext}");
#endif

            var tasksList = new List<BaseTask>();

            foreach (var item in items)
            {
#if DEBUG
                //Info("D99601A0-5F07-417D-921A-0B77E84956AB", $"item = {item}");
#endif

                var task = _tasksStorage.GetBaseTaskByName(Logger, item.Name);

#if DEBUG
                //Info("2A728ED1-B0C8-4FC5-AB66-D978F97A91E3", $"task = {task}");
#endif

                if(task == null)
                {
                    return;
                }

                tasksList.Add(task);
            }

            ReplaceBuiltPlanItems(tasksList, clonnedBuildPlanIterationContext);

            ProcessIteration(tasksPlannerGlobalContext, clonnedBuildPlanIterationContext);

            //throw new NotImplementedException("40A79CD7-9DCB-4B93-BDB2-A6F328E79CA4");
        }

        private void ReplaceBuiltPlanItems(List<BaseTask> tasksList, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("174DE9EE-0069-4F7F-991E-EF8AB0417996", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

            var builtPlanItem = buildPlanIterationContext.TasksToProcess[buildPlanIterationContext.ProcessedIndex];

#if DEBUG
            //Info("8A913405-9ABA-430E-964D-B0223B5BEF0F", $"builtPlanItem = {builtPlanItem}");
#endif

            var parentTasks = builtPlanItem.ParentTasks.ToList();
            parentTasks.Add(builtPlanItem.ProcessedTask);

#if DEBUG
            //Info("57DF1D77-3101-44D3-9F0A-826F0944FA18", $"parentTasks = {parentTasks.WriteListToString()}");
#endif

            var newBuiltPlanItems = new List<BuiltPlanItem>();

            foreach(var task in tasksList)
            {
                newBuiltPlanItems.Add(new BuiltPlanItem
                {
                    ParentTasks = parentTasks.ToList(),
                    ProcessedTask = task
                });
            }

#if DEBUG
            //Info("EE36EF43-6AB6-41D1-8E7E-EC07DDB89BB2", $"newBuiltPlanItems = {newBuiltPlanItems.WriteListToString()}");
#endif

            ReplaceBuiltPlanItems(newBuiltPlanItems, buildPlanIterationContext.TasksToProcess, buildPlanIterationContext.ProcessedIndex);

#if DEBUG
            //Info("B2424A2E-6B2F-4BC7-AC33-46853F5989A2", $"buildPlanIterationContext = {buildPlanIterationContext}");
#endif

            buildPlanIterationContext.ProcessedIndex--;
        }

        private void ReplaceBuiltPlanItems(List<BuiltPlanItem> newBuiltPlanItems, List<BuiltPlanItem> tasksToProcess, int index)
        {
#if DEBUG
            //Info("46D5D5CB-BEDC-4260-8DEC-4BA7E3890FAC", $"index = {index}");
#endif

            if(newBuiltPlanItems.Count == 1)
            {
                tasksToProcess[index] = newBuiltPlanItems[0];
                return;
            }

            throw new NotImplementedException("C7088945-55F6-45D4-A384-38CC03A7C7BC");
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
