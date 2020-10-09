/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedSecondaryRulePart : IndexedBaseRulePart
    {
        public SecondaryRulePart OriginalSecondaryRulePart { get; set; }

        /// <inheritdoc/>
        public override BaseRulePart OriginRulePart => OriginalSecondaryRulePart;

        public IndexedPrimaryRulePart PrimaryPart { get; set; }

        /// <inheritdoc/>
        public override IList<IndexedBaseRulePart> GetNextPartsList()
        {
            return new List<IndexedBaseRulePart>() { PrimaryPart };
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
