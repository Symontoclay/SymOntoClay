/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Compiling.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public abstract class BaseNode: BaseLoggedComponent
    {
        protected BaseNode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        public List<IntermediateScriptCommand> Result => _result;

        protected readonly List<IntermediateScriptCommand> _result = new List<IntermediateScriptCommand>();

        protected void AddCommand(IntermediateScriptCommand command)
        {
            _result.Add(command);
        }

        protected void AddCommands(List<IntermediateScriptCommand> commands)
        {
            _result.AddRange(commands);
        }
        
        protected void CompileVarDecl(IVarDecl varDeclAstExpression)
        {            
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.VarDecl;
            command.AnnotatedItem = varDeclAstExpression;

            AddCommand(command);
        }

        protected void CompilePushVal(Value value, KindOfCompilePushVal kindOfCompilePushVal)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value;

            AddCommand(command);

            var internalKindOfCompilePushValItems = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

#if DEBUG
            //Info("A683D34C-505F-46D2-BEC6-282CA9C075BB", $"internalKindOfCompilePushValItems = {JsonConvert.SerializeObject(internalKindOfCompilePushValItems.Select(p => p.ToString()), Formatting.Indented)}");
#endif
            var internalVarKindOfCompilePushValItems = internalKindOfCompilePushValItems.Where(p => p == InternalKindOfCompilePushVal.DirectVar || p == InternalKindOfCompilePushVal.SetVar || p == InternalKindOfCompilePushVal.GetVar);
            var internalPropKindOfCompilePushValItems = internalKindOfCompilePushValItems.Where(p => p == InternalKindOfCompilePushVal.DirectProp || p == InternalKindOfCompilePushVal.SetProp || p == InternalKindOfCompilePushVal.GetProp);

            InternalKindOfCompilePushVal? internalVarKindOfCompilePushValItem = internalVarKindOfCompilePushValItems.Any() ? internalVarKindOfCompilePushValItems.SingleOrDefault() : (InternalKindOfCompilePushVal?)null;
            InternalKindOfCompilePushVal? internalPropKindOfCompilePushValItem = internalPropKindOfCompilePushValItems.Any() ? internalPropKindOfCompilePushValItems.SingleOrDefault() : (InternalKindOfCompilePushVal?)null;

#if DEBUG
            //Info("41A07939-3F85-45B9-92C0-EE317CD48ED8", $"internalVarKindOfCompilePushValItem  = {internalVarKindOfCompilePushValItem}");
            //Info("DEF74918-104E-4F00-9DD5-C419F0C74DE7", $"internalPropKindOfCompilePushValItem = {internalPropKindOfCompilePushValItem}");
#endif

            switch (value.KindOfValue)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var name = value.AsStrongIdentifierValue;

                        switch (name.KindOfName)
                        {
                            case KindOfName.Var:
                            case KindOfName.SystemVar:
                                if(internalVarKindOfCompilePushValItem.HasValue)
                                {
                                    switch (internalVarKindOfCompilePushValItem)
                                    {
                                        case InternalKindOfCompilePushVal.DirectVar:
                                            break;

                                        case InternalKindOfCompilePushVal.GetVar:
                                            CompileLoadFromVar();
                                            break;

                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(internalVarKindOfCompilePushValItem), internalVarKindOfCompilePushValItem, null);
                                    }
                                }
                                break;

                            case KindOfName.CommonConcept:
                                if(internalPropKindOfCompilePushValItem.HasValue)
                                {
                                    switch (internalPropKindOfCompilePushValItem)
                                    {
                                        case InternalKindOfCompilePushVal.DirectProp:
                                            break;

                                        case InternalKindOfCompilePushVal.GetProp:
                                            CompileTryLoadFromProperty();
                                            break;

                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(internalPropKindOfCompilePushValItem), internalPropKindOfCompilePushValItem, null);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case KindOfValue.PointRefValue:
                    if (internalVarKindOfCompilePushValItem.HasValue || internalPropKindOfCompilePushValItem.HasValue)
                    {
                        //Check TryResolveFromVarOrExpr in the ValueResolvingHelper
                        throw new NotImplementedException("18036216-1223-4190-8DC3-A17FC4522D24");
                    }
                    break;
            }
        }

        private void CompileLoadFromVar()
        {
            var cmd = new IntermediateScriptCommand();
            cmd.OperationCode = OperationCode.LoadFromVar;

            AddCommand(cmd);
        }

        private void CompileTryLoadFromProperty()
        {
            var cmd = new IntermediateScriptCommand();
            cmd.OperationCode = OperationCode.TryLoadFromStrongIdentifier;

            AddCommand(cmd);
        }

#if DEBUG
        public void DbgPrintCommands(string messagePointId)
        {
            var sb = new StringBuilder();

            NDbgPrintCommands(_result, sb);

            Info(messagePointId, $"sb = {sb}");
        }

        public void DbgPrintCommands(string messagePointId, List<IntermediateScriptCommand> intermediateCommandsList)
        {
            var sb = new StringBuilder();

            NDbgPrintCommands(intermediateCommandsList, sb);

            Info(messagePointId, $"sb = {sb}");
        }

        private void NDbgPrintCommands(List<IntermediateScriptCommand> intermediateCommandsList, StringBuilder sb)
        {
            var spaces = DisplayHelper.Spaces(DisplayHelper.IndentationStep);

            sb.AppendLine("Begin Code");

            var n = 0;

            var cmdDict = new Dictionary<IntermediateScriptCommand, int>();

            foreach (var commandItem in intermediateCommandsList)
            {
                cmdDict[commandItem] = n;
                n++;
            }

            n = 0;

            foreach (var commandItem in intermediateCommandsList)
            {

                sb.AppendLine($"{spaces}{n}: {IntermediateScriptCommandToRawDbgString(commandItem, cmdDict)}");

                n++;
            }

            sb.AppendLine("End Code");
        }

        private string IntermediateScriptCommandToRawDbgString(IntermediateScriptCommand commandItem, Dictionary<IntermediateScriptCommand, int> cmdDict)
        {
            var operationCode = commandItem.OperationCode;

            switch (operationCode)
            {
                case OperationCode.Nop:
                case OperationCode.ClearStack:
                case OperationCode.Return:
                case OperationCode.ReturnVal:
                case OperationCode.SetInheritance:
                case OperationCode.SetNotInheritance:
                case OperationCode.Error:
                case OperationCode.RemoveSEHGroup:
                case OperationCode.Await:
                case OperationCode.CompleteAction:
                case OperationCode.CompleteActionVal:
                case OperationCode.BreakAction:
                case OperationCode.BreakActionVal:
                case OperationCode.CancelAction:
                case OperationCode.WeakCancelAction:
                case OperationCode.SetState:
                case OperationCode.SetDefaultState:
                case OperationCode.CompleteState:
                case OperationCode.CompleteStateVal:
                case OperationCode.BreakState:
                case OperationCode.BreakStateVal:
                case OperationCode.Reject:
                case OperationCode.Exec:
                case OperationCode.CodeItemDecl:                
                case OperationCode.CallDefaultCtors:
                case OperationCode.ExecCallEvent:
                case OperationCode.AddLifeCycleEvent:
                case OperationCode.BeginPrimitiveHtnTask:
                case OperationCode.EndPrimitiveHtnTask:
                case OperationCode.VarDecl:
                case OperationCode.PropDecl:
                case OperationCode.LoadFromVar:
                case OperationCode.TryLoadFromStrongIdentifier:
                    return $"{operationCode}";

                case OperationCode.PushVal:
                    return $"{operationCode} {commandItem.Value.ToDbgString()}";

                case OperationCode.CallUnOp:
                case OperationCode.CallBinOp:
                    return $"{operationCode} {OperatorsHelper.GetSymbol(commandItem.KindOfOperator)}";

                case OperationCode.Call:
                case OperationCode.Call_N:
                case OperationCode.Call_P:
                case OperationCode.AsyncCall:
                case OperationCode.AsyncCall_N:
                case OperationCode.AsyncCall_P:
                case OperationCode.CallCtor:
                case OperationCode.CallCtor_N:
                case OperationCode.CallCtor_P:
                case OperationCode.Instantiate:
                case OperationCode.Instantiate_N:
                case OperationCode.Instantiate_P:
                    return $"{operationCode} {commandItem.CountParams}";

                case OperationCode.Wait:
                    return $"{operationCode} {commandItem.CountParams}";

                case OperationCode.SetSEHGroup:
                case OperationCode.JumpTo:
                case OperationCode.JumpToIfTrue:
                case OperationCode.JumpToIfFalse:
                    if (commandItem.JumpToMe == null)
                    {
                        return $"{operationCode} -";
                    }

                    if(cmdDict.ContainsKey(commandItem.JumpToMe))
                    {
                        return $"{operationCode} {cmdDict[commandItem.JumpToMe]}";
                    }

                    return $"{operationCode} *";

                case OperationCode.BeginCompoundHtnTask:
                case OperationCode.EndCompoundHtnTask:
                    return $"{operationCode} {commandItem.CompoundTask?.ToHumanizedLabel()}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(operationCode), operationCode, null);
            }
        }
#endif
    }
}
