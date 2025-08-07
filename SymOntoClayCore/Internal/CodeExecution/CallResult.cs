using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CallResult<T>: VoidCallResult
    {
        public CallResult()
        {
        }

        public CallResult(T value)
        {
            Value = value;
            KindOfResult = KindOfCallResult.Value;
        }

        public T Value { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintPODProp(n, nameof(Value), Value?.ToString());

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintPODProp(n, nameof(Value), Value?.ToString());

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintPODProp(n, nameof(Value), Value?.ToString());

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
