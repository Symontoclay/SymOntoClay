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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
    public class IndexedRuleInstance: IndexedAnnotatedItem
    {
        public RuleInstance Origin { get; set; }

        /// <inheritdoc/>
        public override AnnotatedItem OriginalAnnotatedItem => Origin;

        public IndexedStrongIdentifierValue Name { get; set; }

        public KindOfRuleInstance Kind { get; set; } = KindOfRuleInstance.Undefined;

        public ulong Key { get; set; }

        public bool IsRule { get; set; }
        public IndexedPrimaryRulePart PrimaryPart { get; set; }
        public List<IndexedSecondaryRulePart> SecondaryParts { get; set; } = new List<IndexedSecondaryRulePart>();
        public List<ulong> UsedKeysList { get; set; }

        public void CalculateUsedKeys()
        {
            var usedKeysList = new List<ulong>();

            PrimaryPart.CalculateUsedKeys(usedKeysList);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach (var secondaryPart in SecondaryParts)
                {
                    secondaryPart.CalculateUsedKeys(usedKeysList);
                }
            }

            UsedKeysList = usedKeysList.Distinct().ToList();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ PrimaryPart.GetLongHashCode();

            if(!SecondaryParts.IsNullOrEmpty())
            {
                foreach(var secondaryPart in SecondaryParts)
                {
                    result ^= secondaryPart.GetLongHashCode();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintPODList(n, nameof(UsedKeysList), UsedKeysList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintPODList(n, nameof(UsedKeysList), UsedKeysList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);

            sb.PrintPODList(n, nameof(UsedKeysList), UsedKeysList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            return $"{spaces}{DebugHelperForRuleInstance.ToString(Origin)}";
        }
    }
}
