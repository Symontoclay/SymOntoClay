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
                var executedTask = item.ExecutedTask;

#if DEBUG
                //Info("EE346998-409A-40AE-B91E-0557290EEC43", $"executedTask = {executedTask.ToHumanizedLabel()}");
#endif

                var kindOfPrimitiveTask = executedTask.KindOfPrimitiveTask;

#if DEBUG
                //Info("B5B76ECE-D653-4D41-B0C7-22D59DDCE2CC", $"kindOfPrimitiveTask = {kindOfPrimitiveTask}");
#endif

                switch (kindOfPrimitiveTask)
                {
                    case KindOfPrimitiveTask.BeginCompound:
                        {
                            var command = new IntermediateScriptCommand();
                            command.OperationCode = OperationCode.BeginCompoundHtnTask;

                            var compoundTask = executedTask.AsBeginCompoundHtnTask.CompoundTask;

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
                            //DbgPrintCommands(intermediateCommandsList);
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
                //DbgPrintCommands();
#endif
            }

            //throw new NotImplementedException("D2001943-B737-4140-A864-50DF6E4CC056");
        }
    }
}
