using Newtonsoft.Json.Linq;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues
{
    public class ConstSmartValue<T>: SmartValue<T>
    {
        public ConstSmartValue(T value)
        { 
            _value = value; 
        }

        private readonly T _value;

        /// <inheritdoc/>
        public override T Value => _value;

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
