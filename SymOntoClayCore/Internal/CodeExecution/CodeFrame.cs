using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeFrame : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public CompiledFunctionBody CompiledFunctionBody { get; set; }
        public int CurrentPosition { get; set; }
        public Stack<Value> ValuesStack { get; private set; } = new Stack<Value>();

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

            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            //sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");
            //sb.AppendLine($"{spaces}{nameof(Value)} = {Value}");
            //sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            //sb.AppendLine($"{spaces}{nameof(JumpTo)} = {JumpTo}");
            //sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

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

            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintShortObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            //sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            //sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");
            //sb.AppendLine($"{spaces}{nameof(Value)} = {Value}");
            //sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            //sb.AppendLine($"{spaces}{nameof(JumpTo)} = {JumpTo}");
            //sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

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

            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");

            sb.PrintBriefObjListProp(n, nameof(ValuesStack), ValuesStack.ToList());

            //sb.AppendLine($"{spaces}{nameof(OperationCode)} = {OperationCode}");
            //sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");
            //sb.AppendLine($"{spaces}{nameof(Value)} = {Value}");
            //sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            //sb.AppendLine($"{spaces}{nameof(JumpTo)} = {JumpTo}");
            //sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            return sb.ToString();
        }
    }
}
