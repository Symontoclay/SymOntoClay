/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        public IndexedValue Value { get; set; }
        public ScriptCommand JumpToMe { get; set; }
        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;
        public int CountParams { get; set; }

        public ulong GetLongHashCode()
        {
            ulong result = (ulong)Math.Abs(OperationCode.GetHashCode()) ^ (ulong)Math.Abs(Position.GetHashCode());

            if(Value != null)
            {
                result ^= Value.GetLongHashCode();
            }

            if (JumpToMe != null)
            {
                result ^= JumpToMe.GetLongHashCode();
            }

            result ^= (ulong)Math.Abs(KindOfOperator.GetHashCode()) ^ (ulong)Math.Abs(CountParams.GetHashCode());

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);
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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);
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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.PrintBriefObjProp(n, nameof(JumpToMe), JumpToMe);
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
                case OperationCode.UseInheritance:
                case OperationCode.UseNotInheritance:          
                    return $"{spaces}{OperationCode}";

                case OperationCode.PushVal:
                case OperationCode.PushValFromVar:
                case OperationCode.PushValToVar:
                    return $"{spaces}{OperationCode} {Value.ToDbgString()}";

                case OperationCode.CallUnOp:
                case OperationCode.CallBinOp:
                    return $"{spaces}{OperationCode} {OperatorsHelper.GetSymbol(KindOfOperator)}";

                case OperationCode.AllocateAnonymousWaypoint:
                case OperationCode.AllocateNamedWaypoint:
                    return $"{spaces}{OperationCode} {CountParams}";

                case OperationCode.Call:
                case OperationCode.Call_N:
                case OperationCode.Call_P:
                case OperationCode.AsyncCall:
                case OperationCode.AsyncCall_N:
                case OperationCode.AsyncCall_P:
                    return $"{spaces}{OperationCode} {CountParams}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(OperationCode), OperationCode, null);
            }
        }
    }
}
