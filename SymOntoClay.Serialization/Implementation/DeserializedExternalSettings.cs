using NLog;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class DeserializedExternalSettings : IDeserializedExternalSettings
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public void RegExternalSettings(object settings, Type settingsType, Type holderType, string holderKey)
        {
#if DEBUG
            _logger.Info($"settings = {settings}");
            _logger.Info($"settingsType.FullName = {settingsType.FullName}");
            _logger.Info($"holderType.FullName = {holderType.FullName}");
            _logger.Info($"holderKey = {holderKey}");
#endif

            if (_externalSettingsDict.TryGetValue(holderType, out var holderKeysDict))
            {
                if(holderKeysDict.TryGetValue(holderKey, out var settingsTypeDict))
                {
                    settingsTypeDict[settingsType] = settings;
                }
                else
                {
                    settingsTypeDict = new Dictionary<Type, object>();
                    settingsTypeDict[settingsType] = settings;

                    holderKeysDict[holderKey] = settingsTypeDict;
                }
            }
            else
            {
                var settingsTypeDict = new Dictionary<Type, object>();
                settingsTypeDict[settingsType] = settings;

                holderKeysDict = new Dictionary<string, Dictionary<Type, object>>();
                holderKeysDict[holderKey] = settingsTypeDict;

                _externalSettingsDict[holderType] = holderKeysDict;
            }
        }

        private Dictionary<Type, Dictionary<string, Dictionary<Type, object>>> _externalSettingsDict = new Dictionary<Type, Dictionary<string, Dictionary<Type, object>>>();
    }
}
