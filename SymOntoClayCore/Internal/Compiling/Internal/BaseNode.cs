/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
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
        
        protected void CompileValue(Value value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            if(value.IsStrongIdentifierValue)
            {
                var name = value.AsStrongIdentifierValue;

                var kindOfName = name.KindOfName;

                switch(kindOfName)
                {
                    case KindOfName.Concept:
                    case KindOfName.Channel:
                    case KindOfName.Entity:
                        CompilePushVal(value);
                        break;

                    case KindOfName.SystemVar:
                    case KindOfName.Var:
                        CompilePushVal(value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                }

                return;
            }

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value;

            AddCommand(command);
        }

        protected void CompileVarDecl(IVarDecl varDeclAstExpression)
        {
            CompilePushVal(varDeclAstExpression.Name);

            foreach (var typeItem in varDeclAstExpression.TypesList)
            {
                CompilePushVal(typeItem);
            }
            
            CompilePushAnnotation(varDeclAstExpression);

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.VarDecl;
            command.CountParams = varDeclAstExpression.TypesList.Count;

            AddCommand(command);
        }

        protected void CompilePushVal(Value value)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value;

            AddCommand(command);
        }

        protected void CompilePushAnnotation(IAnnotatedItem annotatedItem)
        {
            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = annotatedItem.GetAnnotationValue();

            AddCommand(command);
        }

#if DEBUG
        public void DbgPrintCommands()
        {
            var spaces = DisplayHelper.Spaces(DisplayHelper.IndentationStep);

            var sb = new StringBuilder();
            sb.AppendLine("Begin Code");

            var n = 0;

            var cmdDict = new Dictionary<IntermediateScriptCommand, int>();
            
            foreach (var commandItem in _result)
            {
                cmdDict[commandItem] = n;
                n++;
            }

            n = 0;

            foreach (var commandItem in _result)
            {
                //Log($"commandItem = {commandItem}");

                sb.AppendLine($"{spaces}{n}: {IntermediateScriptCommandToRawDbgString(commandItem, cmdDict)}");

                n++;
            }

            sb.AppendLine("End Code");

            Log($"sb = {sb}");
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

                case OperationCode.VarDecl:
                    return $"{operationCode} {commandItem.CountParams}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(operationCode), operationCode, null);
            }
        }
#endif
    }
}
