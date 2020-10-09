/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

        public IndexedInheritanceItem Indexed { get; set; }

        public IndexedInheritanceItem GetIndexed(IMainStorageContext mainStorageContext)
        {
            if(Indexed == null)
            {
                return ConvertorToIndexed.ConvertInheritanceItem(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertInheritanceItem(this, mainStorageContext, convertingContext);
            }

            return Indexed;
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
            sb.PrintExisting(n, nameof(Indexed), Indexed);
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
