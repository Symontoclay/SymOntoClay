using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization.SmartValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal.SmartValues
{
    public class MotitorNodeSettingsDictionarySmartValue : SmartValue<BaseMonitorSettings>
    {
        public MotitorNodeSettingsDictionarySmartValue()
        {
        }

        public MotitorNodeSettingsDictionarySmartValue(SmartValue<IDictionary<string, BaseMonitorSettings>> nodesSettings,
            SmartValue<BaseMonitorSettings> baseMonitorSettings,
            SmartValue<bool> enableOnlyDirectlySetUpNodes, string nodeId, bool isCache)
        {
            _nodesSettings = nodesSettings;
            _baseMonitorSettings = baseMonitorSettings;
            _enableOnlyDirectlySetUpNodes = enableOnlyDirectlySetUpNodes;
            _nodeId = nodeId;
            _isCache = isCache;
        }

        private readonly SmartValue<IDictionary<string, BaseMonitorSettings>> _nodesSettings;
        private readonly SmartValue<BaseMonitorSettings> _baseMonitorSettings;
        private readonly SmartValue<bool> _enableOnlyDirectlySetUpNodes;
        private SmartValue<BaseMonitorSettings> _nodeSettings;
        private readonly string _nodeId;

        private readonly bool _isCache;
        private bool _isCalculated;
        private BaseMonitorSettings _calculatedValue = default;

        /// <inheritdoc/>
        public override BaseMonitorSettings Value
        {
            get
            {
                if (_isCache)
                {
                    if (_isCalculated)
                    {
                        return _calculatedValue;
                    }
                }

                var nodesSettingsDict = _nodesSettings.Value;

                if (nodesSettingsDict.ContainsKey(_nodeId))
                {
                    _calculatedValue = nodesSettingsDict[_nodeId];

                    _isCalculated = true;

                    return _calculatedValue;
                }

                _nodeSettings = new CloneBaseMonitorSettingsSmartValue(_baseMonitorSettings, true);

                if (_enableOnlyDirectlySetUpNodes.Value)
                {
                    _nodeSettings.Value.Enable = false;
                }

                _calculatedValue = _nodeSettings.Value;

                _isCalculated = true;

                return _calculatedValue;
            }
        }

        /// <inheritdoc/>
        public override void SetValue(BaseMonitorSettings value)
        {
            throw new NotSupportedException("C4773E05-E895-4127-9B2A-B6D1A979F7AA");
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_nodesSettings), _nodesSettings);
            sb.PrintObjProp(n, nameof(_baseMonitorSettings), _baseMonitorSettings);
            sb.PrintObjProp(n, nameof(_enableOnlyDirectlySetUpNodes), _enableOnlyDirectlySetUpNodes);
            sb.AppendLine($"{spaces}{nameof(_nodeId)} = {_nodeId}");
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjProp(n, nameof(_nodesSettings), _nodesSettings);
            sb.PrintShortObjProp(n, nameof(_baseMonitorSettings), _baseMonitorSettings);
            sb.PrintShortObjProp(n, nameof(_enableOnlyDirectlySetUpNodes), _enableOnlyDirectlySetUpNodes);
            sb.AppendLine($"{spaces}{nameof(_nodeId)} = {_nodeId}");
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(_nodesSettings), _nodesSettings);
            sb.PrintBriefObjProp(n, nameof(_baseMonitorSettings), _baseMonitorSettings);
            sb.PrintBriefObjProp(n, nameof(_enableOnlyDirectlySetUpNodes), _enableOnlyDirectlySetUpNodes);
            sb.AppendLine($"{spaces}{nameof(_nodeId)} = {_nodeId}");
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
