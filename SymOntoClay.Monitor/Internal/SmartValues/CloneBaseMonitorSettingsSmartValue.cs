using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization.SmartValues;
using System.Text;
using System;

namespace SymOntoClay.Monitor.Internal.SmartValues
{
    public class CloneBaseMonitorSettingsSmartValue : SmartValue<BaseMonitorSettings>
    {
        public CloneBaseMonitorSettingsSmartValue()
        { 
        }

        public CloneBaseMonitorSettingsSmartValue(SmartValue<BaseMonitorSettings> source, bool isCache)
        {
            _source = source;
            _isCache = isCache;
        }

        private readonly SmartValue<BaseMonitorSettings> _source;
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

                _calculatedValue = _source.Value.Clone();
                _isCalculated = true;

                return _calculatedValue;
            }
        }

        /// <inheritdoc/>
        public override void SetValue(BaseMonitorSettings value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_source), _source);
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjProp(n, nameof(_source), _source);
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(_source), _source);
            sb.AppendLine($"{spaces}{nameof(_isCache)} = {_isCache}");
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
