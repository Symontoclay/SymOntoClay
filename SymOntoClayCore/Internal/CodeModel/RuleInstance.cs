using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstance: AnnotatedItem
    {
        public string DictionaryName { get; set; }
        public Name Name { get; set; }
        public PrimaryRulePart PrimaryPart { get; set; }
        public List<SecondaryRulePart> SecondaryParts { get; set; } = new List<SecondaryRulePart>();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);
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

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);
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

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
