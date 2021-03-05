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

using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InheritanceItem: AnnotatedItem
    {
        public InheritanceItem()
        {
            Id = NameHelper.GetNewEntityNameString();
        }

        public string Id { get; set; }

        public StrongIdentifierValue SubName { get; set; } = new StrongIdentifierValue();

        /// <summary>
        /// Represents ancestor.
        /// </summary>
        public StrongIdentifierValue SuperName { get; set; } = new StrongIdentifierValue();

        /// <summary>
        /// Represents rank of inheritance between two objects.
        /// It must be resolved to LogicalValue.
        /// </summary>
        [ResolveToType(typeof(LogicalValue))]
        public Value Rank { get; set; }
        public bool IsSystemDefined { get; set; }

        public IList<string> KeysOfPrimaryRecords { get; set; } = new List<string>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ SubName.GetLongHashCode() ^ SuperName.GetLongHashCode();

            if (Rank != null)
            {
                result ^= LongHashCodeWeights.BaseModalityWeight ^ Rank.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public InheritanceItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public InheritanceItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (InheritanceItem)context[this];
            }

            var result = new InheritanceItem();
            context[this] = result;
            result.SubName = SubName.Clone(context);
            result.SuperName = SuperName.Clone(context);
            result.Rank = Rank.CloneValue(context);
            result.IsSystemDefined = IsSystemDefined;
            result.KeysOfPrimaryRecords = KeysOfPrimaryRecords;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            SubName?.DiscoverAllAnnotations(result);
            SuperName?.DiscoverAllAnnotations(result);
            Rank?.DiscoverAllAnnotations(result);
        }

        private void PrintHeader(StringBuilder sb, uint n, string spaces)
        {
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.PrintObjProp(n, nameof(SubName), SubName);
            sb.PrintObjProp(n, nameof(SuperName), SuperName);
            sb.PrintObjProp(n, nameof(Rank), Rank);
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            PrintHeader(sb, n, spaces);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
