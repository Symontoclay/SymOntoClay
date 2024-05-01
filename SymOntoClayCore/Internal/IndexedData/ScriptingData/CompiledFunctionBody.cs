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
using SymOntoClay.Core.Internal.CodeExecution;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData.ScriptingData
{
    public class CompiledFunctionBody : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public Dictionary<int, ScriptCommand> Commands { get; set; } = new Dictionary<int, ScriptCommand>();
        public Dictionary<int, SEHGroup> SEH { get; set; } = new Dictionary<int, SEHGroup>();

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompiledFunctionBody Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompiledFunctionBody Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompiledFunctionBody)context[this];
            }

            var result = new CompiledFunctionBody();
            context[this] = result;

            result.Commands = Commands?.ToDictionary(p => p.Key, p => p.Value.Clone(context));
            result.SEH = SEH?.ToDictionary(p => p.Key, p => p.Value.Clone(context));

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
            var sb = new StringBuilder();

            sb.PrintObjDict_2_Prop(n, nameof(Commands), Commands);
            sb.PrintObjDict_2_Prop(n, nameof(SEH), SEH);

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

            sb.PrintShortObjDict_2_Prop(n, nameof(Commands), Commands);
            sb.PrintShortObjDict_2_Prop(n, nameof(SEH), SEH);

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

            sb.PrintBriefObjDict_2_Prop(n, nameof(Commands), Commands);
            sb.PrintBriefObjDict_2_Prop(n, nameof(SEH), SEH);

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
            var sb = new StringBuilder();

            foreach(var commandItem in Commands)
            {
                sb.AppendLine($"{spaces}{commandItem.Key}: {commandItem.Value.ToDbgString()}");
            }

            return sb.ToString();
        }
    }
}
