/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
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
        public LocalCodeExecutionContext LocalContext { get; set; }
        public ProcessInfo ProcessInfo { get; set; }
        public CodeEntity Metadata { get; set; }

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

            sb.PrintObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintObjProp(n, nameof(Metadata), Metadata);

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

            sb.PrintShortObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintShortObjProp(n, nameof(Metadata), Metadata);

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

            sb.PrintBriefObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintExisting(n, nameof(ProcessInfo), ProcessInfo);

            sb.PrintBriefObjProp(n, nameof(Metadata), Metadata);

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
