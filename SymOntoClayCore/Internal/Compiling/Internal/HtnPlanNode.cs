/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
                //Info("F6C74D37-1435-4671-AEB7-5FA074768947", $"item = {item}");
#endif

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
                            var compoundTask = executedTask.AsBeginCompoundHtnTask.CompoundTask;

                            if(compoundTask.PreconditionExpression != null)
                            {
                                var node = new ExpressionNode(_context);
                                node.Run(compoundTask.PreconditionExpression);

                                AddCommands(node.Result);

                                var checkingCommand = new IntermediateScriptCommand()
                                {
                                    OperationCode = OperationCode.CheckHtnTaskCondition
                                };

                                AddCommand(checkingCommand);

#if DEBUG
                                //DbgPrintCommands("20359737-C0C2-4FA4-B5A0-33176A4E72FB");
#endif
                            }

                            var conditionExpression = item.TaskCase?.ConditionExpression;

                            if (conditionExpression != null)
                            {
                                var node = new ExpressionNode(_context);
                                node.Run(conditionExpression);

                                AddCommands(node.Result);

                                var checkingCommand = new IntermediateScriptCommand()
                                {
                                    OperationCode = OperationCode.CheckHtnTaskCondition
                                };

                                AddCommand(checkingCommand);

#if DEBUG
                                //DbgPrintCommands("5013AF10-7F21-4FF8-9194-702D361B435C");
#endif
                            }

#if DEBUG
                            //DbgPrintCommands("D01C8253-BFE2-415D-9610-D58C989C6BDE");
#endif

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
                //DbgPrintCommands("D9DA1696-6D70-4D83-82C0-48AC1B879454");
#endif
            }

            //throw new NotImplementedException("D2001943-B737-4140-A864-50DF6E4CC056");
        }
    }
}
