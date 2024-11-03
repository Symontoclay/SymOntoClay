using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues
{
    public class ExternalSettingsSmartValuePlainObject : IObjectToString
    {
        public string SettingType { get; set; }
        public string HolderType { get; set; }
        public string HolderKey { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(SettingType)} = {SettingType}");
            sb.AppendLine($"{spaces}{nameof(HolderType)} = {HolderType}");
            sb.AppendLine($"{spaces}{nameof(HolderKey)} = {HolderKey}");
            return sb.ToString();
        }
    }
}
