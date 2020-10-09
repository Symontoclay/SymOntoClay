/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedWaypointValue : IndexedValue
    {
        public WaypointValue OriginalWaypointValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalWaypointValue;

        /// <inheritdoc/>
        public override bool IsWaypointValue => true;

        /// <inheritdoc/>
        public override IndexedWaypointValue AsWaypointValue => this;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.WaypointValue;

        public Vector3 AbcoluteCoordinates { get; set; }
        public IndexedStrongIdentifierValue Name { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return AbcoluteCoordinates;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode() ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ (ulong)Math.Abs(AbcoluteCoordinates.GetHashCode());
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(AbcoluteCoordinates)} = {AbcoluteCoordinates}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        protected override string PropertiesToDbgString(uint n)
        {
            if(Name != null && !Name.IsEmpty)
            {
                return $"{Name.NameValue}[{AbcoluteCoordinates}]";
            }

            return $"#@[{AbcoluteCoordinates}]";
        }
    }
}
