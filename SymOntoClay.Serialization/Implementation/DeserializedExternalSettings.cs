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
        public void RegExternalSettings(object settings, Type holderType, string holderKey)
        {
#if DEBUG
            _logger.Info($"settings = {settings}");
            _logger.Info($"holderType.FullName = {holderType.FullName}");
            _logger.Info($"holderKey = {holderKey}");
#endif

            if (_externalSettingsDict.TryGetValue(holderType, out var holderKeysDict))
            {
                holderKeysDict[holderKey] = settings;
            }
            else
            {
                _externalSettingsDict[holderType] = new Dictionary<string, object>
                {
                    { holderKey, settings }
                };
            }
        }

        private Dictionary<Type, Dictionary<string, object>> _externalSettingsDict = new Dictionary<Type, Dictionary<string, object>>();
    }
}
