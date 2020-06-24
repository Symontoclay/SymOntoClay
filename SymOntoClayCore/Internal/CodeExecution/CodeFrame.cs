using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeFrame : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public CompiledFunctionBody CompiledFunctionBody { get; set; }
        public int CurrentPosition { get; set; }
        public Stack<Value> ValuesStack { get; private set; } = new Stack<Value>();
        public IStorage Storage { get; set; }

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
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}Begin Code");

            foreach (var commandItem in CompiledFunctionBody.Commands)
            {
                var currentMark = string.Empty;

                if(commandItem.Key == CurrentPosition)
                {
                    currentMark = "-> ";
                }

                sb.AppendLine($"{nextNSpaces}{currentMark}{commandItem.Key}: {commandItem.Value.ToDbgString()}");
            }
            sb.AppendLine($"{spaces}End Code");
            sb.AppendLine($"{spaces}Begin Values Stack");
            foreach(var stackItem in ValuesStack)
            {
                sb.AppendLine($"{nextNSpaces}{stackItem.ToDbgString()}");
            }
            sb.AppendLine($"{spaces}End Values Stack");
            return sb.ToString();
        }
    }
}
