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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeFrameEvnPart : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public ILocalCodeExecutionContext LocalContext { get; set; }
        public CodeItem Metadata { get; set; }
        public IInstance Instance { get; set; }
        public IExecutionCoordinator ExecutionCoordinator { get; set; }
        public IInstance CompoundTaskInstance { get; set; }

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

            sb.PrintObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintObjProp(n, nameof(Metadata), Metadata);
            sb.PrintObjProp(n, nameof(Instance), Instance);
            sb.PrintObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.PrintObjProp(n, nameof(CompoundTaskInstance), CompoundTaskInstance);

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

            sb.PrintShortObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintShortObjProp(n, nameof(Metadata), Metadata);
            sb.PrintShortObjProp(n, nameof(Instance), Instance);
            sb.PrintShortObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.PrintShortObjProp(n, nameof(CompoundTaskInstance), CompoundTaskInstance);

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

            sb.PrintBriefObjProp(n, nameof(LocalContext), LocalContext);

            sb.PrintBriefObjProp(n, nameof(Metadata), Metadata);
            sb.PrintBriefObjProp(n, nameof(Instance), Instance);
            sb.PrintBriefObjProp(n, nameof(ExecutionCoordinator), ExecutionCoordinator);
            sb.PrintBriefObjProp(n, nameof(CompoundTaskInstance), CompoundTaskInstance);

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
            var nextN = n + DisplayHelper.IndentationStep;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();
            
            if (Instance != null)
            {
                sb.AppendLine($"{spaces}Instance: {Instance.Name.NameValue}");
            }

            if (ExecutionCoordinator != null)
            {
                sb.AppendLine($"{spaces}ExecutionCoordinator: {ExecutionCoordinator.Id}");
            }

            if (Metadata != null)
            {
                var holder = Metadata.Holder;

                if (holder != null)
                {
                    sb.AppendLine($"{spaces}Holder: {holder.NameValue}");
                }

                sb.AppendLine($"{spaces}{Metadata.ToHumanizedLabel()}");
            }

            if (CompoundTaskInstance != null)
            {
                sb.AppendLine($"{spaces}CompoundTask Instance: {CompoundTaskInstance.Name.NameValue}");
            }

            return sb.ToString();
        }
    }
}
