using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues.Functors
{
    public class FieldSmartValue<S, T> : SmartValue<T>
    {
        public FieldSmartValue() 
        {
        }

        public FieldSmartValue(SmartValue<S> source, string settingsPropertyName, bool isCache)
            : this(source, new List<string> { settingsPropertyName }, isCache)
        {
        }

        public FieldSmartValue(SmartValue<S> source, IEnumerable<string> settingsPropertyName, bool isCache)
        {
            _source = source;
            _settingsPropertyName = settingsPropertyName;
            _isCache = isCache;
        }

        private readonly SmartValue<S> _source;
        private readonly IEnumerable<string> _settingsPropertyName;

        private readonly bool _isCache;
        private bool _isCalculated;
        private T _calculatedValue = default;

        /// <inheritdoc/>
        public override T Value
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

                object targetObject = _source.Value;

                if(targetObject == null)
                {
                    _calculatedValue = default(T);

                    return _calculatedValue;
                }

                var targetType = typeof(S);

                foreach (var itemName in _settingsPropertyName)
                {
#if DEBUG
                    //_logger.Info($"itemName = {itemName}");
#endif

                    var property = targetType.GetProperty(itemName);

#if DEBUG
                    //_logger.Info($"property?.Name = {property?.Name}");
#endif

                    if (property != null)
                    {
                        targetObject = property.GetValue(targetObject);

#if DEBUG
                        //_logger.Info($"targetObject = {targetObject}");
#endif

                        if (targetObject == null)
                        {
                            _calculatedValue = default(T);

                            return _calculatedValue;
                        }

                        targetType = targetObject.GetType();

                        continue;
                    }

                    var field = targetType.GetField(itemName);

#if DEBUG
                    //_logger.Info($"field?.Name = {field?.Name}");
#endif

                    if (field != null)
                    {
                        targetObject = field.GetValue(targetObject);

#if DEBUG
                        //_logger.Info($"targetObject = {targetObject}");
#endif

                        if (targetObject == null)
                        {
                            _calculatedValue = default(T);

                            return _calculatedValue;
                        }

                        targetType = targetObject.GetType();

                        continue;
                    }

                    throw new NotImplementedException("8D4CAA77-978B-4AD0-AB4B-8DCAE6A187AC");
                }

#if DEBUG
                //_logger.Info($"targetObject = {targetObject}");
#endif

                _calculatedValue = (T)targetObject;

                return _calculatedValue;
            }
        }

        /// <inheritdoc/>
        public override void SetValue(T value)
        {
            throw new NotSupportedException("32605D46-2D8F-4D6B-99C8-133A0150FA75");
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_source), _source);
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjProp(n, nameof(_source), _source);
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(_source), _source);
            sb.PrintPODList(n, nameof(_settingsPropertyName), _settingsPropertyName);
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
