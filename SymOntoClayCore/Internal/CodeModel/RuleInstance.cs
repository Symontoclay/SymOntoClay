/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: AnnotatedItem
    {
        public string DictionaryName { get; set; }
        public StrongIdentifierValue Name { get; set; }
        public KindOfRuleInstance Kind { get; set; } = KindOfRuleInstance.Undefined;
        public PrimaryRulePart PrimaryPart { get; set; }
        public IList<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();
        public IList<string> KeysOfPrimaryRecords { get; set; } = new List<string>();

        public IndexedRuleInstance Indexed { get; set; }

        public IndexedRuleInstance GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertRuleInstance(this, mainStorageContext);
            }

            return Indexed;
        }

        public IndexedRuleInstance GetIndexed(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertRuleInstance(this, mainStorageContext, convertingContext);
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
                return ConvertorToIndexed.ConvertRuleInstance(this, mainStorageContext, convertingContext);
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
        public RuleInstance Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public RuleInstance Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RuleInstance)context[this];
            }

            var result = new RuleInstance();
            context[this] = result;

            result.DictionaryName = DictionaryName;
            result.Name = Name.Clone(context);
            result.Kind = Kind;
            result.PrimaryPart = PrimaryPart.Clone(context);
            result.SecondaryParts = SecondaryParts.Select(p => p.Clone(context)).ToList();
            result.KeysOfPrimaryRecords = KeysOfPrimaryRecords?.ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            PrimaryPart?.DiscoverAllAnnotations(result);

            if (!SecondaryParts.IsNullOrEmpty())
            {
                foreach(var item in SecondaryParts)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        public IList<LogicalQueryNode> GetInheritanceRelations()
        {
            if(Kind != KindOfRuleInstance.Fact)
            {
                return new List<LogicalQueryNode>();
            }

            return PrimaryPart.GetInheritanceRelations();            
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
