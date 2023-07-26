/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageSettings: IObjectToString
    {
        public IMainStorageContext MainStorageContext { get; set; }
        public IList<IStorage> ParentsStorages { get; set; }
        public ILocalCodeExecutionContext ParentCodeExecutionContext { get; set; }
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }
        public IInheritancePublicFactsReplicator InheritancePublicFactsReplicator { get; set; }
        public KindOfGC KindOfGC { get; set; } = KindOfGC.None;
        public bool EnableOnAddingFactEvent { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsIsolated { get; set; }

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
            sb.PrintExisting(n, nameof(MainStorageContext), MainStorageContext);
            sb.PrintExisting(n, nameof(ParentCodeExecutionContext), ParentCodeExecutionContext);
            sb.PrintObjListProp(n, nameof(ParentsStorages), ParentsStorages);
            sb.PrintObjProp(n, nameof(DefaultSettingsOfCodeEntity), DefaultSettingsOfCodeEntity);
            sb.PrintExisting(n, nameof(InheritancePublicFactsReplicator), InheritancePublicFactsReplicator);
            sb.AppendLine($"{spaces}{nameof(KindOfGC)} = {KindOfGC}");
            sb.AppendLine($"{spaces}{nameof(EnableOnAddingFactEvent)} = {EnableOnAddingFactEvent}");
            sb.AppendLine($"{spaces}{nameof(Enabled)} = {Enabled}");
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            return sb.ToString();
        }
    }
}
