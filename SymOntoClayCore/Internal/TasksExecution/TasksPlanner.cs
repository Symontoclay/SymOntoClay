using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Instances;
using System;
using System.Collections.Generic;
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

            throw new NotImplementedException("FF8CD857-079E-49A1-8C06-32D475C38D56");
        }

        private List<BaseCompoundTask> GetRootTasks()
        {
#if DEBUG
            Info("47323362-D3B3-47FD-B346-D746771DB8C7", $"_mainEntity.Name = {_mainEntity.Name}");
            Info("600EDBC3-9F4F-43AE-B900-C029F4BB1AEC", $"_mainEntity.GetType().Name = {_mainEntity.GetType().Name}");
            Info("E40E6248-2A1C-4CF7-95AB-BBC82924C46E", $"_mainEntity.RootTasks = {_mainEntity.RootTasks.WriteListToString()}");
#endif

            var result = new List<BaseCompoundTask>();

            var mainEntityRootTasksNames = _mainEntity.RootTasks;

            if(!mainEntityRootTasksNames.IsNullOrEmpty())
            {
                foreach(var taskName in mainEntityRootTasksNames)
                {
                    var task = _tasksStorage.GetBaseCompoundTaskByName(Logger, taskName);

#if DEBUG
                    Info("627C7805-7915-4787-BB76-E7D2B2B7EE56", $"task = {task}");
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
