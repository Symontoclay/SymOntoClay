using NLog;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues
{
    public abstract class SmartValue<T> : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
#if DEBUG
        protected static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public abstract T Value { get; }

        public abstract void SetValue(T value);

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            var value = Value;

            var toStringValue = value as IObjectToString;

            if(toStringValue == null)
            {
                sb.AppendLine($"{spaces}{nameof(Value)} = {value}");
            }
            else
            {
                sb.PrintObjProp(n, nameof(Value), toStringValue);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            var value = Value;

            var toStringValue = value as IObjectToShortString;

            if (toStringValue == null)
            {
                sb.AppendLine($"{spaces}{nameof(Value)} = {value}");
            }
            else
            {
                sb.PrintShortObjProp(n, nameof(Value), toStringValue);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            var value = Value;

            var toStringValue = value as IObjectToBriefString;

            if (toStringValue == null)
            {
                sb.AppendLine($"{spaces}{nameof(Value)} = {value}");
            }
            else
            {
                sb.PrintBriefObjProp(n, nameof(Value), toStringValue);
            }

            return sb.ToString();
        }
    }
}
