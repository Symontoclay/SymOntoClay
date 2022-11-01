using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution.Helpers
{
    public class ConversionExecutableToCodeFrameAdditionalSettings: IObjectToString
    {
        public long? Timeout { get; set; }
        public float? Priority { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Timeout)} = {Timeout}");
            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

            return sb.ToString();
        }
    }
}
