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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class LocalCodeExecutionContext: ILocalCodeExecutionContext
    {
        public LocalCodeExecutionContext()
        {
        }

        public LocalCodeExecutionContext(ILocalCodeExecutionContext parent)
        {
            Parent = parent;
        }

        /// <inheritdoc/>
        public ILocalCodeExecutionContext Parent { get;private set; }

        /// <inheritdoc/>
        public bool UseParentInResolving { get; set; } = true;

        /// <inheritdoc/>
        public bool IsIsolated { get; set; }

        /// <inheritdoc/>
        public StrongIdentifierValue Holder { get; set; }

        /// <inheritdoc/>
        public IStorage Storage { get; set; }

        /// <inheritdoc/>
        public StrongIdentifierValue Owner { get; set; }

        /// <inheritdoc/>
        public IStorage OwnerStorage { get; set; }

        /// <inheritdoc/>
        public KindOfLocalCodeExecutionContext Kind { get; set; } = KindOfLocalCodeExecutionContext.Usual;

        /// <inheritdoc/>
        public KindOfAddFactOrRuleResult KindOfAddFactResult { get; set; }

        /// <inheritdoc/>
        public MutablePartOfRuleInstance MutablePart { get; set; }

        /// <inheritdoc/>
        public RuleInstance AddedRuleInstance { get; set; }

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
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}"); 
            sb.AppendLine($"{spaces}{nameof(IsIsolated)} = {IsIsolated}");
            sb.PrintObjProp(n, nameof(Holder), Holder);            
            sb.AppendLine($"{spaces}{nameof(Storage)}.Kind = {Storage?.Kind}");
            sb.PrintObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.Kind = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.TargetClassName = {OwnerStorage?.TargetClassName?.NameValue}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintObjProp(n, nameof(MutablePart), MutablePart);
            sb.PrintObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);
            
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
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}");
            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(Storage)}.Kind = {Storage?.Kind}");
            sb.PrintShortObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.Kind = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.TargetClassName = {OwnerStorage?.TargetClassName?.NameValue}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintShortObjProp(n, nameof(MutablePart), MutablePart);
            sb.PrintShortObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);

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
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(UseParentInResolving)} = {UseParentInResolving}");
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            sb.AppendLine($"{spaces}{nameof(Storage)}.Kind = {Storage?.Kind}");
            sb.PrintBriefObjProp(n, nameof(Owner), Owner);
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.Kind = {OwnerStorage?.Kind}");
            sb.AppendLine($"{spaces}{nameof(OwnerStorage)}.TargetClassName = {OwnerStorage?.TargetClassName?.NameValue}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfAddFactResult)} = {KindOfAddFactResult}");
            sb.PrintBriefObjProp(n, nameof(MutablePart), MutablePart);
            sb.PrintBriefObjProp(n, nameof(AddedRuleInstance), AddedRuleInstance);

            return sb.ToString();
        }
    }
}
