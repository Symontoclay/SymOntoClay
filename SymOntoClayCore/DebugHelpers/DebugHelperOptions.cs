using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public class DebugHelperOptions : IObjectToString
    {
        public HumanizedOptions HumanizedOptions { get; set; } = HumanizedOptions.ShowAll;
        public bool IsHtml { get; set; }
        public List<IObjectToString> ItemsForSelection { get; set; }
        public MutablePartOfRuleInstance MutablePartOfRuleInstance { get; set; }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public DebugHelperOptions Clone()
        {
            var result = new DebugHelperOptions();

            result.HumanizedOptions = HumanizedOptions;
            result.IsHtml = IsHtml;
            result.ItemsForSelection = ItemsForSelection?.ToList();
            result.MutablePartOfRuleInstance = MutablePartOfRuleInstance;

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HumanizedOptions)} = {HumanizedOptions}");
            sb.AppendLine($"{spaces}{nameof(IsHtml)} = {IsHtml}");
            sb.PrintObjListProp(n, nameof(ItemsForSelection), ItemsForSelection);
            sb.PrintObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);            

            return sb.ToString();
        }
    }
}
