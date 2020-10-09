/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SecondaryRulePart: BaseRulePart
    {
        public PrimaryRulePart PrimaryPart { get; set; }

        public IndexedSecondaryRulePart Indexed { get; set; }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        public IndexedSecondaryRulePart GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                ConvertorToIndexed.ConvertRuleInstance(Parent, mainStorageContext);
            }

            return Indexed;
        }

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
                ConvertorToIndexed.ConvertRuleInstance(Parent, mainStorageContext, convertingContext);
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
        public SecondaryRulePart Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public SecondaryRulePart Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (SecondaryRulePart)context[this];
            }

            var result = new SecondaryRulePart();
            context[this] = result;

            result.PrimaryPart = PrimaryPart.Clone(context);

            result.AppendBaseRulePart(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
