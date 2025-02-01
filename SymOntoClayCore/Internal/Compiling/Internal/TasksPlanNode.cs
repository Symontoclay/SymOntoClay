using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.TasksExecution;
using System;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class TasksPlanNode : BaseNode
    {
        public TasksPlanNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(TasksPlan plan)
        {
            foreach(var item in plan.Items)
            {
                var executedTask = item.ExecutedTask;

#if DEBUG
                Info("EE346998-409A-40AE-B91E-0557290EEC43", $"executedTask = {executedTask.ToHumanizedLabel()}");
#endif

                var kindOfPrimitiveTask = executedTask.KindOfPrimitiveTask;

#if DEBUG
                Info("B5B76ECE-D653-4D41-B0C7-22D59DDCE2CC", $"kindOfPrimitiveTask = {kindOfPrimitiveTask}");
#endif

                switch (kindOfPrimitiveTask)
                {
                    case KindOfPrimitiveTask.BeginCompound:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.BeginCompoundTask;
                            command.CompoundTask = executedTask.AsBeginCompoundTask.CompoundTask;

                            AddCommand(command);
                        }
                        break;

                    case KindOfPrimitiveTask.Primitive:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.BeginPrimitiveTask;
                            AddCommand(command);

                            var compiledFunctionBody = executedTask.AsPrimitiveTask.Operator.CompiledFunctionBody;

#if DEBUG
                            Info("AE5A761D-A7EF-4C9B-A993-C6BB2A8048BE", $"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");
#endif

                            foreach(var cmd in compiledFunctionBody.Commands)
                            {
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfPrimitiveTask), kindOfPrimitiveTask, null);
                }

#if DEBUG
                DbgPrintCommands();
#endif
            }

            throw new NotImplementedException("D2001943-B737-4140-A864-50DF6E4CC056");
        }
    }
}
