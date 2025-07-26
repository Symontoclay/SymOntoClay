using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Htn
{
    public class HtnPlanner: BaseComponent
    {
        public HtnPlanner(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
            _mainEntity = context.InstancesStorage.MainEntity;
            _tasksStorage = context.Storage.GlobalStorage.TasksStorage;
            _compiler = context.Compiler;
        }

        private readonly IEngineContext _context;
        private readonly AppInstance _mainEntity;
        private readonly ITasksStorage _tasksStorage;
        private ICompiler _compiler;

        public bool HasRootTasks => _mainEntity == null ? false : GetRootTasks().Count > 0;

        public HtnPlan BuildPlan()
        {
            if(_mainEntity == null)
            {
                return HtnPlan.EmptyPlan;
            }

            var rootTasks = GetRootTasks();

#if DEBUG
            //Info("54AF89ED-3EBB-4A31-86A9-577EE1AF5190", $"rootTasks = {rootTasks.WriteListToString()}");
#endif

            if(rootTasks.Count == 0)
            {
                return HtnPlan.EmptyPlan;
            }

            if(rootTasks.Count > 1)
            {
                throw new NotImplementedException("0A44F791-B382-4B29-8F48-B62EB772014E");
            }

            //TODO: make processing multiple root task for working with voice commands.
            var rootTask = rootTasks.Single();

            var tasksPlannerGlobalContext = new HtnPlannerGlobalContext();

            return BuildPlan(tasksPlannerGlobalContext, rootTask);
        }

        private static List<StrongIdentifierValue> _previousRootTasksEmptyList = new List<StrongIdentifierValue>();

        private HtnPlan BuildPlan(HtnPlannerGlobalContext tasksPlannerGlobalContext, BaseHtnTask rootTask)
        {
            return BuildPlan(tasksPlannerGlobalContext, rootTask, _previousRootTasksEmptyList);
        }

        private HtnPlan BuildPlan(HtnPlannerGlobalContext tasksPlannerGlobalContext, BaseHtnTask rootTask, List<StrongIdentifierValue> previousRootTasks)
        {
            return BuildPlan(tasksPlannerGlobalContext, rootTask, previousRootTasks, new List<BaseHtnTask> { rootTask });
        }

        private HtnPlan BuildPlan(HtnPlannerGlobalContext tasksPlannerGlobalContext, BaseHtnTask rootTask, List<StrongIdentifierValue> previousRootTasks, List<BaseHtnTask> processedTasks)
        {
            var rootTaskName = rootTask.Name;

            var allRootTasks = new List<StrongIdentifierValue>();
            allRootTasks.AddRange(previousRootTasks);
            allRootTasks.Add(rootTaskName);

            var buildPlanIterationContext = new BuildPlanIterationContext()
            {
                PreviousRootTasks = previousRootTasks,
                RootTask = rootTaskName,
                AllRootTasks = allRootTasks
            };

            buildPlanIterationContext.LocalCodeExecutionContext = new BuildPlanIterationLocalCodeExecutionContext(_context, _context.InstancesStorage.MainEntity.LocalCodeExecutionContext);
            
            foreach(var processedTask in processedTasks)
            {
                buildPlanIterationContext.TasksToProcess.Add(new BuiltPlanItem
                {
                    ProcessedTask = processedTask
                });
            }

#if DEBUG
            //Info("1E36D336-39B2-4548-BFCD-12E072D51755", $"buildPlanIterationContext = {buildPlanIterationContext}");
            //Info("D531DC15-FE0B-4134-AA25-953997E60221", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

            ProcessIteration(tasksPlannerGlobalContext, buildPlanIterationContext);

#if DEBUG
            //Info("75115FF7-6822-4AB7-AE76-633B44127845", $"tasksPlannerGlobalContext = {tasksPlannerGlobalContext}");
#endif

            var completedIterations = tasksPlannerGlobalContext.CompletedIterations;

            if (completedIterations.Count == 0)
            {
                return HtnPlan.EmptyPlan;
            }

            var targetCompletedIteration = GetTargetCompletedIteration(completedIterations);

            return ConvertCompletedIterationToTasksPlan(targetCompletedIteration);
        }

        private HtnPlan ConvertCompletedIterationToTasksPlan(BuildPlanIterationContext completedIteration)
        {
#if DEBUG
            //Info("0ACC770A-B3CD-4182-8E86-4D7CF4F660D5", $"completedIteration = {completedIteration}");
#endif

            var result = new HtnPlan();

            var builtItems = new List<HtnPlanItem>();
            result.Items = builtItems;

            foreach (var builtItem in completedIteration.TasksToProcess)
            {
#if DEBUG
                //Info("EA3F4844-5C18-4F00-8C30-8BDB6BAD6DF2", $"builtItem = {builtItem.ToDbgString()}");
#endif

                var planItem = new HtnPlanItem()
                {
                    ExecutedTask = builtItem.ProcessedTask.AsBasePrimitiveHtnTask
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
            var filteredCompletedIterations = new List<BuildPlanIterationContext>();

            foreach (var completedIteration in completedIterations)
            {
                if(TryResolveBackground(completedIteration))
                {
                    filteredCompletedIterations.Add(completedIteration);
                }
            }

#if DEBUG
            //foreach (var item in filteredCompletedIterations)
            //{
            //    Info("0DAE374E-968B-49C1-879C-EA62BC8CA4CB", $"item = {item.ToDbgString()}");
            //}
#endif

            return filteredCompletedIterations.FirstOrDefault();
        }

        private bool TryResolveBackground(BuildPlanIterationContext completedIteration)
        {
#if DEBUG
            //Info("78AACDEE-639D-4F24-B5A6-46858255F991", $"completedIteration = {completedIteration.ToDbgString()}");
#endif

            var tasksWithBackground = completedIteration.TasksWithBackground;

            if (tasksWithBackground.IsNullOrEmpty())
            {
                return true;
            }

            foreach (var task in tasksWithBackground)
            {
                if(!TryResolveBackground(task, completedIteration))
                {
                    completedIteration.MarkAsFailed();
                    return false;
                }
            }

            return true;
        }

        private bool TryResolveBackground(BaseCompoundHtnTask task, BuildPlanIterationContext completedIteration)
        {
#if DEBUG
            //Info("907C3ACD-58A6-4A63-AA46-D7507E892FF0", $"task = {task}");
#endif

            foreach(var item in task.Backgrounds)
            {
                if(!TryResolveBackground(task, item, completedIteration))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryResolveBackground(BaseCompoundHtnTask task, CompoundHtnTaskBackground background, BuildPlanIterationContext completedIteration)
        {
#if DEBUG
            //Info("86FC3019-CBF5-4AD5-AA4D-0E20BD12536E", $"background = {background}");
#endif

            var tasksPlannerGlobalContext = new HtnPlannerGlobalContext();

            var tasksList = ConvertHtnCaseItemsToHtnTasks(background.Items, task.KindOfTask);

#if DEBUG
            //Info("E385666D-485F-40B3-A53E-D7AFA67873B1", $"tasksList.Count = {tasksList.Count}");
#endif

            var backgroundPlan = BuildPlan(tasksPlannerGlobalContext, task, completedIteration.AllRootTasks, tasksList);

#if DEBUG
            //Info("12B59D03-DD9C-4196-B8F8-E3DB3F4DE1F9", $"backgroundPlan = {backgroundPlan.ToDbgString()}");
#endif

            if(backgroundPlan.IsEmpty)
            {
                return false;
            }

            background.Plan = backgroundPlan;
            background.CompiledFunctionBody = _compiler.Compile(backgroundPlan);

#if DEBUG
            //Info("D6D35AE8-6082-4133-961E-CB359DBCA1D1", $"background.CompiledFunctionBody = {background.CompiledFunctionBody.ToDbgString()}");
#endif

            return true;
        }

        private enum PrepareBuildPlanIterationContextState
        {
            Init,
            WasJump
        }

        private BuildPlanIterationContext PrepareBuildPlanIterationContext(BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("7774FEE2-D085-4388-88E1-373D4E1D03EC", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

            var tasksToProcess = buildPlanIterationContext.TasksToProcess;

            if (!tasksToProcess.Any(p => p.ProcessedTask.IsJumpPrimitiveHtnTask))
            {
                return buildPlanIterationContext;
            }

            var state = PrepareBuildPlanIterationContextState.Init;

            BuiltPlanItem jumpBuiltItem = null;
            var currentIndex = 0;

            while(true)
            {
#if DEBUG
                //Info("E2A7188A-D480-42CB-8914-FF4C818D96C7", $"currentIndex = {currentIndex}");
                //Info("1E64FDA7-89D5-4ABC-81BB-CDE15EA724A9", $"tasksToProcess.Count = {tasksToProcess.Count}");
                //Info("370C5A7F-8D17-4629-872C-56C03D91C918", $"state = {state}");
                //Info("2F4BDDD2-A71A-47FF-BAF8-12EA348B6E2F", $"jumpBuiltItem = {jumpBuiltItem?.ToDbgString()}");
                //Info("EF4F54AC-A238-48F0-9FCD-16C97B176615", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

                if (currentIndex >= tasksToProcess.Count)
                {
                    break;
                }

                var currentItem = tasksToProcess[currentIndex];

#if DEBUG
                //Info("9BF47F56-D67B-412E-A599-49FFDFE870CB", $"currentItem = {currentItem.ToDbgString()}");
#endif

                var currentTask = currentItem.ProcessedTask;

                var kindOfProcessedTask = currentTask.KindOfTask;

#if DEBUG
                //Info("DECBE823-98EE-4A2B-8EB4-FE0790A29B8D", $"kindOfProcessedTask = {kindOfProcessedTask}");
#endif

                switch (state)
                {
                    case PrepareBuildPlanIterationContextState.Init:
                        switch(kindOfProcessedTask)
                        {
                            case KindOfTask.BeginCompound:
                            case KindOfTask.EndCompound:
                            case KindOfTask.Primitive:
                                currentIndex++;
                                break;

                            case KindOfTask.Jump:
                                jumpBuiltItem = currentItem;
                                tasksToProcess.Remove(jumpBuiltItem);
                                state = PrepareBuildPlanIterationContextState.WasJump;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfProcessedTask), kindOfProcessedTask, null);
                        }
                        break;

                    case PrepareBuildPlanIterationContextState.WasJump:
                        switch(kindOfProcessedTask)
                        {
                            case KindOfTask.Primitive:
                                tasksToProcess.Remove(currentItem);
                                break;

                            case KindOfTask.EndCompound:
                                {
                                    var targetIndex = currentIndex + 1;

#if DEBUG
                                    //Info("7C5C45A5-A08A-4D21-85DA-E770559E8864", $"targetIndex = {targetIndex}");
#endif

                                    if (currentIndex >= tasksToProcess.Count)
                                    {
                                        tasksToProcess.Add(jumpBuiltItem);
                                        currentIndex++;
                                    }
                                    else
                                    {
                                        tasksToProcess.Insert(targetIndex, jumpBuiltItem);
                                        currentIndex += 2;
                                    }
                                    
#if DEBUG
                                    //Info("CC9C16D8-EBB6-482A-B187-0263A2932ACA", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

                                    jumpBuiltItem = null;

                                    state = PrepareBuildPlanIterationContextState.Init;
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfProcessedTask), kindOfProcessedTask, null);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }

            return buildPlanIterationContext;
        }

        private void ProcessIteration(HtnPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("FDD9D703-6231-450A-9B55-AA2734952558", "Begin");
#endif

            while(true)
            {
#if DEBUG
                //Info("8BA1B85B-A1DF-4ABB-9CE4-925E8190303A", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
                //Info("2CD7257B-DEA0-4C75-83B4-5F037BA1DDA0", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

                if (buildPlanIterationContext.ProcessedIndex == -1)
                {
                    buildPlanIterationContext.ProcessedIndex++;
                    continue;
                }

#if DEBUG
                //Info("C168FD2F-C8E4-4090-B6AA-0D212F6D2947", $"buildPlanIterationContext.TasksToProcess.Count = {buildPlanIterationContext.TasksToProcess.Count}");
#endif

                var tasksToProcess = buildPlanIterationContext.TasksToProcess;

#if DEBUG
                //Info("636F6B73-4499-4A09-9E76-70D7012A288D", $"tasksToProcess = {tasksToProcess.WriteListToString()}");
#endif

                if (buildPlanIterationContext.ProcessedIndex == tasksToProcess.Count)
                {
#if DEBUG
                    //Info("B98C9500-A74D-411F-B6BF-0F1FB98EA58D", $"buildPlanIterationContext.ProcessedIndex == tasksToProcess.Count");
#endif

                    if (buildPlanIterationContext.IsNormal && tasksToProcess.All(p => p.ProcessedTask.IsBasePrimitiveHtnTask))
                    {
                        tasksPlannerGlobalContext.CompletedIterations.Add(PrepareBuildPlanIterationContext(buildPlanIterationContext));
                    }

                    return;
                }

                var currentProcessedTask = tasksToProcess[buildPlanIterationContext.ProcessedIndex].ProcessedTask;

                if (currentProcessedTask.IsBasePrimitiveHtnTask)
                {
                    if(currentProcessedTask.IsPrimitiveTask)
                    {
                        var currentProcessedPrimitiveTask = currentProcessedTask.AsPrimitiveTask;

                        if(!CheckCondition(currentProcessedPrimitiveTask.Precondition, buildPlanIterationContext))
                        {
                            buildPlanIterationContext.MarkAsFailed();
                            return;
                        }
                    }

                    buildPlanIterationContext.ProcessedIndex++;
                    continue;
                }
#if DEBUG
                //Info("ABDE6F0C-CA9B-49DB-9377-DB3092F19827", $"buildPlanIterationContext.ProcessedIndex (after) = {buildPlanIterationContext.ProcessedIndex}");
#endif

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
                    case KindOfTask.Root:
                    case KindOfTask.Strategic:
                    case KindOfTask.Tactical:
                    case KindOfTask.Compound:
                        ProcessBaseCompoundTask(currentBuiltPlanItem, tasksPlannerGlobalContext, buildPlanIterationContext);
                        break;

                    case KindOfTask.EndCompound:
                        break;

                    case KindOfTask.Replaced:
                        return;

                    case KindOfTask.Primitive:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfCurrentTask), kindOfCurrentTask, null);
                }

                //throw new NotImplementedException("2CDDF950-725E-45EC-8D3B-5BD2684F77FD");
            }
        }


        private void ProcessBaseCompoundTask(BuiltPlanItem builtPlanItem, HtnPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("DF3B0700-5B7E-4101-BB8B-FF159ADF9080", "Begin");
#endif

            var processedTask = builtPlanItem.ProcessedTask.AsBaseCompoundHtnTask;

#if DEBUG
            //Info("35B5E17A-C30E-4EF7-91F6-66D1F5E9950A", $"processedTask = {processedTask.ToHumanizedLabel()}");
            //Info("09D1E8B4-09D0-4C0F-826F-20B50B016019", $"processedTask = {processedTask}");
#endif

            var processedTaskName = processedTask.Name;

            if (buildPlanIterationContext.VisitedCompoundTasks.Contains(processedTaskName))
            {
                ReplaceBuiltPlanItems(new List<BaseHtnTask> { new JumpPrimitiveHtnTask() { TargetTaskName = processedTaskName } }, buildPlanIterationContext);

                return;
            }

            if(buildPlanIterationContext.PreviousRootTasks.Contains(processedTaskName))
            {
                buildPlanIterationContext.MarkAsFailed();
                return;
            }

            var beginCompoundTask = new BeginCompoundHtnTask() 
            { 
                CompoundTask = processedTask 
            };

            ReplaceBuiltPlanItems(new List<BaseHtnTask> 
                {
                    beginCompoundTask,
                    new ReplacedCompoundHtnTask() { CompoundTask = processedTask },
                    new EndCompoundHtnTask() { CompoundTask = processedTask } 
                },
                buildPlanIterationContext);

            buildPlanIterationContext.ProcessedIndex++;

            buildPlanIterationContext.VisitedCompoundTasks.Add(processedTaskName);

#if DEBUG
            //Info("7182C96F-E323-4F08-A053-EC7BD7345219", $"buildPlanIterationContext (--) = {buildPlanIterationContext}");
            //Info("BD768CC6-94F8-4888-B29D-3C8373751DD1", $"buildPlanIterationContext (--) = {buildPlanIterationContext.ToDbgString()}");
#endif

            //if(processedTask.IsStrategicTask)
            //{
            //throw new NotImplementedException("8C0447E9-6893-413E-9607-4CEBEC748519");
            //}

            if(!processedTask.Backgrounds.IsNullOrEmpty())
            {
                buildPlanIterationContext.TasksWithBackground.Add(processedTask);
            }

            var hasApprovedConditionalCase = false;

            foreach (var taskCase in processedTask.Cases)
            {
#if DEBUG
                //Info("0BEBD584-A2F2-496E-800B-E04E6F5F7CED", $"taskCase = {taskCase}");
#endif

                if(!CheckTaskCase(taskCase, buildPlanIterationContext))
                {
#if DEBUG
                    //Info("5487F697-56DD-4B80-AD8D-D1D3167A80C1", "!CheckTaskCase");
#endif

                    continue;
                }

                if(taskCase.Condition == null)
                {
                    if(hasApprovedConditionalCase)
                    {
                        continue;
                    }
                }
                else
                {
                    hasApprovedConditionalCase = true;
                }

                ProcessTaskCase(taskCase, processedTask, processedTask.KindOfTask, tasksPlannerGlobalContext, buildPlanIterationContext);
            }

            //throw new NotImplementedException("20A515FC-9D9F-4185-B14E-12C80C5CFCDD");
        }

        private bool CheckTaskCase(CompoundHtnTaskCase taskCase, BuildPlanIterationContext buildPlanIterationContext)
        {
            return CheckCondition(taskCase.Condition, buildPlanIterationContext);
        }

        private bool CheckCondition(LogicalExecutableExpression condition, BuildPlanIterationContext buildPlanIterationContext)
        {
            if (condition == null)
            {
                return true;
            }

            var value = _context.CodeExecutor.CallExecutableSync(Logger, condition, null, buildPlanIterationContext.LocalCodeExecutionContext, CallMode.Default);

#if DEBUG
            //Info("00BD9C17-3ACC-4E69-BA9A-FCD2DC2B8175", $"value = {value}");
#endif

            return LogicalValue.TrueValue.Equals(value);
        }

        private void ProcessTaskCase(CompoundHtnTaskCase taskCase, BaseCompoundHtnTask processedTask, KindOfTask requestingKindOfTask, HtnPlannerGlobalContext tasksPlannerGlobalContext, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("90913386-6F54-47D4-B1D6-EC49F29604FC", "Begin");
#endif

            var caseItems = taskCase.Items;

            if (caseItems.IsNullOrEmpty())
            {
                return;
            }

            List<CompoundHtnTaskCaseItem> items = null;

            if (processedTask.Before == null && processedTask.After == null)
            {
                items = caseItems;
            }
            else
            {
                items = new List<CompoundHtnTaskCaseItem>();

                if (processedTask.Before != null)
                {
                    items.AddRange(processedTask.Before.Items);
                }

                items.AddRange(caseItems);

                if(processedTask.After != null)
                {
                    items.AddRange(processedTask.After.Items);
                }
            }

#if DEBUG
            //Info("BE38C2B1-B314-454D-936B-7C0DF5A1EAD9", $"buildPlanIterationContext = {buildPlanIterationContext}");
#endif

            var clonedBuildPlanIterationContext = buildPlanIterationContext.Clone();

#if DEBUG
            //Info("FB034078-4FD7-4A5E-9BF4-37EB9C32E75D", $"clonedBuildPlanIterationContext = {clonedBuildPlanIterationContext.ToDbgString()}");
            //Info("BB7CE2CE-D819-4035-8B8D-F32FF8A105FA", $"clonedBuildPlanIterationContext = {clonedBuildPlanIterationContext}");
            //Info("ABD7A72A-2562-46EA-A42A-74CB6136D89D", $"requestingKindOfTask = {requestingKindOfTask}");
#endif

            var tasksList = ConvertHtnCaseItemsToHtnTasks(items, requestingKindOfTask);

            if(tasksList.IsNullOrEmpty())
            {
                return;
            }

            ReplaceBuiltPlanItems(tasksList, clonedBuildPlanIterationContext);

            ProcessIteration(tasksPlannerGlobalContext, clonedBuildPlanIterationContext);

            //throw new NotImplementedException("40A79CD7-9DCB-4B93-BDB2-A6F328E79CA4");
        }

        private List<BaseHtnTask> ConvertHtnCaseItemsToHtnTasks(List<CompoundHtnTaskCaseItem> items, KindOfTask requestingKindOfTask)
        {
            var tasksList = new List<BaseHtnTask>();

            foreach (var item in items)
            {
#if DEBUG
                //Info("D99601A0-5F07-417D-921A-0B77E84956AB", $"item = {item}");
#endif

                var task = _tasksStorage.GetBaseTaskByName(Logger, item.Name, requestingKindOfTask);

#if DEBUG
                //Info("2A728ED1-B0C8-4FC5-AB66-D978F97A91E3", $"task = {task}");
#endif

                if (task == null)
                {
                    return null;
                }

                tasksList.Add(task);
            }

            return tasksList;
        }

        private void ReplaceBuiltPlanItems(List<BaseHtnTask> tasksList, BuildPlanIterationContext buildPlanIterationContext)
        {
#if DEBUG
            //Info("3F772C3E-5E6D-4AB2-B1D2-085AF8589B62", $"buildPlanIterationContext.TasksToProcess.Count = {buildPlanIterationContext.TasksToProcess.Count}");
            //Info("174DE9EE-0069-4F7F-991E-EF8AB0417996", $"buildPlanIterationContext.ProcessedIndex = {buildPlanIterationContext.ProcessedIndex}");
#endif

            var builtPlanItem = buildPlanIterationContext.TasksToProcess[buildPlanIterationContext.ProcessedIndex];

#if DEBUG
            //Info("8A913405-9ABA-430E-964D-B0223B5BEF0F", $"builtPlanItem = {builtPlanItem}");
#endif

            var newBuiltPlanItems = new List<BuiltPlanItem>();

            foreach(var task in tasksList)
            {
                newBuiltPlanItems.Add(new BuiltPlanItem
                {
                    ProcessedTask = task
                });
            }

#if DEBUG
            //Info("EE36EF43-6AB6-41D1-8E7E-EC07DDB89BB2", $"newBuiltPlanItems = {newBuiltPlanItems.WriteListToString()}");
#endif

            ReplaceBuiltPlanItems(newBuiltPlanItems, buildPlanIterationContext.TasksToProcess, buildPlanIterationContext.ProcessedIndex);

#if DEBUG
            //Info("B2424A2E-6B2F-4BC7-AC33-46853F5989A2", $"buildPlanIterationContext = {buildPlanIterationContext.ToDbgString()}");
#endif

            //buildPlanIterationContext.ProcessedIndex--;
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

#if DEBUG
            //Info("C0DBD29A-0A46-4A4B-AAD8-384E2F259D29", $"newBuiltPlanItems = {newBuiltPlanItems.WriteListToString()}");
            //Info("C10A6C8F-50C1-4C14-B070-5995AF34E507", $"tasksToProcess = {tasksToProcess.WriteListToString()}");
            //Info("03D510A6-5683-4B36-B9CB-BEF6EBF288BD", $"tasksToProcess.Count = {tasksToProcess.Count}");
#endif

            List<BuiltPlanItem> oldTasksToProcess = null;

            if(index < tasksToProcess.Count - 1)
            {
                oldTasksToProcess = tasksToProcess.ToList();
            }

            var n = index;

            foreach (var item in newBuiltPlanItems)
            {
                PutBuiltPlanItemToPosition(item, n, tasksToProcess);

                n++;
            }

#if DEBUG
            //Info("4FE2C5E7-A17C-4B4C-BCF1-5EAAC04A8119", $"tasksToProcess (after) = {tasksToProcess.WriteListToString()}");
            //Info("9A3797DD-C976-4A3A-BFE2-CB76567D8F3B", $"tasksToProcess.Count (after) = {tasksToProcess.Count}");
#endif

            if(oldTasksToProcess != null)
            {
                for(var i = index + 1; i < oldTasksToProcess.Count; i++)
                {
#if DEBUG
                    //Info("84448900-C8F8-44AE-9D64-10E3E0193561", $"i = {i}");
                    //Info("4FEE5B45-CDF4-4044-925A-662BA87522FC", $"n = {n}");
#endif

                    PutBuiltPlanItemToPosition(oldTasksToProcess[i], n, tasksToProcess);

                    n++;
                }

                //throw new NotImplementedException("C7088945-55F6-45D4-A384-38CC03A7C7BC");
            }
        }

        private void PutBuiltPlanItemToPosition(BuiltPlanItem item, int index, List<BuiltPlanItem> tasksToProcess)
        {
#if DEBUG
            //Info("2859F8BD-B5F8-41D9-B8AA-2AE09F56B115", $"index = {index}");
            //Info("5B2EAA94-5C53-4ED8-85DC-AA6382E28145", $"tasksToProcess.Count = {tasksToProcess.Count}");
#endif

            if(index >= tasksToProcess.Count)
            {
                tasksToProcess.Add(item);
            }
            else
            {
                tasksToProcess[index] = item;
            }
            
            //throw new NotImplementedException("FF3B8DCF-DAAF-473D-96E8-A38359E75BBB");
        }

        private List<BaseCompoundHtnTask> GetRootTasks()
        {
#if DEBUG
            //Info("47323362-D3B3-47FD-B346-D746771DB8C7", $"_mainEntity.Name = {_mainEntity.Name}");
            //Info("600EDBC3-9F4F-43AE-B900-C029F4BB1AEC", $"_mainEntity.GetType().Name = {_mainEntity.GetType().Name}");
            //Info("E40E6248-2A1C-4CF7-95AB-BBC82924C46E", $"_mainEntity.RootTasks = {_mainEntity.RootTasks.WriteListToString()}");
#endif

            var mainEntityRootTasksNames = _mainEntity.RootTasks;

            if(mainEntityRootTasksNames.IsNullOrEmpty())
            {
                return _tasksStorage.GetAllRootTasks(Logger).Cast<BaseCompoundHtnTask>().ToList();
            }

            var result = new List<BaseCompoundHtnTask>();

            foreach (var taskName in mainEntityRootTasksNames)
            {
                var task = _tasksStorage.GetBaseCompoundTaskByName(Logger, taskName, KindOfTask.Root);

#if DEBUG
                //Info("627C7805-7915-4787-BB76-E7D2B2B7EE56", $"task = {task}");
#endif

                if (task == null)
                {
                    continue;
                }

                result.Add(task);
            }

            return result;
        }
    }
}
