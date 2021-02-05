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
