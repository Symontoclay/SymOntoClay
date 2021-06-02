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
        public CodeScope MainCodeScope { get; set; }
        public CodeScope CurrentCodeScope { get; set; }
        public Stack<CodeScope> CodeScopesStack { get; private set; } = new Stack<CodeScope>();
        public Stack<SEHGroup> SEHStack { get; private set; } = new Stack<SEHGroup>();        
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
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(MainCodeScope), MainCodeScope);
            sb.PrintObjProp(n, nameof(CurrentCodeScope), CurrentCodeScope);

            sb.PrintObjListProp(n, nameof(CodeScopesStack), CodeScopesStack.ToList());
            sb.PrintObjListProp(n, nameof(SEHStack), SEHStack.ToList());

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
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(MainCodeScope), MainCodeScope);
            sb.PrintShortObjProp(n, nameof(CurrentCodeScope), CurrentCodeScope);

            sb.PrintShortObjListProp(n, nameof(CodeScopesStack), CodeScopesStack.ToList());
            sb.PrintShortObjListProp(n, nameof(SEHStack), SEHStack.ToList());

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
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(MainCodeScope), MainCodeScope);
            sb.PrintBriefObjProp(n, nameof(CurrentCodeScope), CurrentCodeScope);

            sb.PrintBriefObjListProp(n, nameof(CodeScopesStack), CodeScopesStack.ToList());
            sb.PrintBriefObjListProp(n, nameof(SEHStack), SEHStack.ToList());

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
            return CurrentCodeScope.ToDbgString(n);
        }
    }
}
