using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedPrimaryRulePart: IndexedBaseRulePart
    {
        public PrimaryRulePart OriginalPrimaryRulePart { get; set; }

        /// <inheritdoc/>
        public override BaseRulePart OriginRulePart => OriginalPrimaryRulePart;

        public List<IndexedSecondaryRulePart> SecondaryParts { get; set; } = new List<IndexedSecondaryRulePart>();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
