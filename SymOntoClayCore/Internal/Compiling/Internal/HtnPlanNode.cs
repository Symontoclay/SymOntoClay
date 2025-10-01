using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Htn;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class HtnPlanNode : BaseNode
    {
        public HtnPlanNode(IMainStorageContext context)
            : base(context)
        {
        }
        
        public void Run(HtnPlan plan)
        {
            var beginTaskItems = new Dictionary<StrongIdentifierValue, IntermediateScriptCommand>();

            foreach (var item in plan.Items)
            {
#if DEBUG
                Info("F6C74D37-1435-4671-AEB7-5FA074768947", $"item = {item}");
#endif

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
                            var compoundTask = executedTask.AsBeginCompoundHtnTask.CompoundTask;

                            if(compoundTask.Precondition != null)
                            {
                                throw new NotImplementedException("13721482-43BF-42D3-968E-E5707BD3B8A4");
                            }

                            if(item.TaskCase != null)
                            {
                                throw new NotImplementedException("213CFBB1-26A6-4076-A6E5-1EECF9E88362");
                            }

                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.BeginCompoundHtnTask;

                            command.CompoundTask = compoundTask;

                            AddCommand(command);

                            beginTaskItems[compoundTask.Name] = command;
                        }
                        break;

                    case KindOfPrimitiveTask.Primitive:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.BeginPrimitiveHtnTask;
                            AddCommand(command);

                            var intermediateCommandsList = executedTask.AsPrimitiveTask.Operator.IntermediateCommandsList;

#if DEBUG
                            //DbgPrintCommands("74C0C437-9332-454A-9FDE-B4F23B09AE1C", intermediateCommandsList);
#endif

                            foreach (var intermediateCommand in intermediateCommandsList)
                            {
                                AddCommand(intermediateCommand);
                            }

                            command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.EndPrimitiveHtnTask;
                            AddCommand(command);
                        }
                        break;

                    case KindOfPrimitiveTask.EndCompound:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.EndCompoundHtnTask;
                            command.CompoundTask = executedTask.AsEndCompoundHtnTask.CompoundTask;

                            AddCommand(command);
                        }
                        break;

                    case KindOfPrimitiveTask.Jump:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.JumpTo;
                            command.JumpToMe = beginTaskItems[executedTask.AsJumpPrimitiveHtnTask.TargetTaskName];

                            AddCommand(command);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfPrimitiveTask), kindOfPrimitiveTask, null);
                }

#if DEBUG
                DbgPrintCommands("D9DA1696-6D70-4D83-82C0-48AC1B879454");
#endif
            }

            //throw new NotImplementedException("D2001943-B737-4140-A864-50DF6E4CC056");
        }
    }
}
