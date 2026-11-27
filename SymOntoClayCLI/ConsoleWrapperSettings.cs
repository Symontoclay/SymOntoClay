using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.CLI
{
    public class ConsoleWrapperSettings : IObjectToString
    {
        public bool WriteOutputToTextFileAsParallel { get; set; }
        public bool UseNLogLogger { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(WriteOutputToTextFileAsParallel)} = {WriteOutputToTextFileAsParallel}");
            sb.AppendLine($"{spaces}{nameof(UseNLogLogger)} = {UseNLogLogger}");

            return sb.ToString();
        }
    }
}
