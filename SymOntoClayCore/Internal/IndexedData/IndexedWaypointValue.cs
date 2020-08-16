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
    }
}
