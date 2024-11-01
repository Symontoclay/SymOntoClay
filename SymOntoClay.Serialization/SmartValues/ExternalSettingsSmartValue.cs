using Newtonsoft.Json;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues
{
    public class ExternalSettingsSmartValue<T>: SmartValue<T>, INonGenericExternalSettingsSmartValue
    {
        public ExternalSettingsSmartValue()
        {
        }

        public ExternalSettingsSmartValue(T value, Type settingType, string settingsPropertyName, Type holderType, string holderKey)
            : this(value, settingType, new List<string> { settingsPropertyName}, holderType, holderKey)
        {
        }

        public ExternalSettingsSmartValue(T value, Type settingType, IEnumerable<string> settingsPropertyName, Type holderType, string holderKey) 
        {
#if DEBUG
            _logger.Info($"value = {value}");
            _logger.Info($"settingType?.FullName = {settingType?.FullName}");
            _logger.Info($"settingsPropertyName = {JsonConvert.SerializeObject(settingsPropertyName, Formatting.Indented)}");
            _logger.Info($"holderType?.FullName = {holderType?.FullName}");
            _logger.Info($"holderKey = {holderKey}");
#endif

            _value = value;
            _settingType = settingType;
            _settingsPropertyName = settingsPropertyName;
            _holderType = holderType;
            _holderKey = holderKey;
        }

        [SocNoSerializable]
        private readonly T _value;


        private readonly Type _settingType;
        private readonly IEnumerable<string> _settingsPropertyName;
        private readonly Type _holderType;
        private readonly string _holderKey;

        /// <inheritdoc/>
        public override T Value => _value;

        ExternalSettingsSmartValuePlainObject INonGenericExternalSettingsSmartValue.GetPlainObject()
        {
            return new ExternalSettingsSmartValuePlainObject
            {
                SettingType = _settingType?.FullName,
                SettingsPropertyName = _settingsPropertyName.ToList(),
                HolderType = _holderType?.FullName,
                HolderKey = _holderKey
            };
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(_settingType)} = {_settingType?.FullName}");
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.AppendLine($"{spaces}{nameof(_holderType)} = {_holderType?.FullName}");
            sb.AppendLine($"{spaces}{nameof(_holderKey)} = {_holderKey}");
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(_settingType)} = {_settingType?.FullName}");
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.AppendLine($"{spaces}{nameof(_holderType)} = {_holderType?.FullName}");
            sb.AppendLine($"{spaces}{nameof(_holderKey)} = {_holderKey}");
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(_settingType)} = {_settingType?.FullName}");
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.AppendLine($"{spaces}{nameof(_holderType)} = {_holderType?.FullName}");
            sb.AppendLine($"{spaces}{nameof(_holderKey)} = {_holderKey}");
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
