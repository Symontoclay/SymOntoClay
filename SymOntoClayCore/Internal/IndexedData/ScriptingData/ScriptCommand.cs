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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData.ScriptingData
{
    public class ScriptCommand : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public OperationCode OperationCode { get; set; } = OperationCode.Nop;

        public int Position { get; set; }
        public Value Value { get; set; }
        public BaseCompoundTask CompoundTask { get; set; }
        public int TargetPosition { get; set; }
        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;
        public int CountParams { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public ScriptCommand Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public ScriptCommand Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ScriptCommand)context[this];
            }

            var result = new ScriptCommand();
            context[this] = result;

            result.OperationCode = OperationCode;
            result.Position = Position;
            result.Value = Value?.CloneValue(context);
            result.CompoundTask = CompoundTask?.CloneBaseCompoundTask(context);
            result.TargetPosition = TargetPosition;
            result.KindOfOperator = KindOfOperator;
            result.CountParams = CountParams;

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.PrintObjProp(n, nameof(CompoundTask), CompoundTask);

            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }
        
        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.PrintShortObjProp(n, nameof(CompoundTask), CompoundTask);

            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.PrintBriefObjProp(n, nameof(CompoundTask), CompoundTask);

            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            switch(OperationCode)
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
                case OperationCode.BeginPrimitiveTask:
                case OperationCode.EndPrimitiveTask:
                    return $"{spaces}{OperationCode}";

                case OperationCode.PushVal:
                    return $"{spaces}{OperationCode} {Value.ToDbgString()}";

                case OperationCode.CallUnOp:
                case OperationCode.CallBinOp:
                    return $"{spaces}{OperationCode} {OperatorsHelper.GetSymbol(KindOfOperator)}";

                case OperationCode.Call:
                case OperationCode.Call_N:
                case OperationCode.Call_P:
                case OperationCode.AsyncCall:
                case OperationCode.AsyncCall_N:
                case OperationCode.AsyncCall_P:
                case OperationCode.AsyncChildCall:
                case OperationCode.AsyncChildCall_N:
                case OperationCode.AsyncChildCall_P:
                case OperationCode.CallCtor:
                case OperationCode.CallCtor_N:
                case OperationCode.CallCtor_P:
                case OperationCode.Instantiate:
                case OperationCode.Instantiate_N:
                case OperationCode.Instantiate_P:
                    return $"{spaces}{OperationCode} {CountParams}";

                case OperationCode.Wait:
                    return $"{spaces}{OperationCode} {CountParams}";

                case OperationCode.SetSEHGroup:
                case OperationCode.JumpTo:
                case OperationCode.JumpToIfTrue:
                case OperationCode.JumpToIfFalse:
                    return $"{spaces}{OperationCode} {TargetPosition}";

                case OperationCode.VarDecl:
                    return $"{spaces}{OperationCode} {CountParams}";

                case OperationCode.BeginCompoundTask:
                case OperationCode.EndCompoundTask:
                    return $"{spaces}{OperationCode} {CompoundTask?.ToHumanizedLabel()}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(OperationCode), OperationCode, null);
            }
        }
    }
}
