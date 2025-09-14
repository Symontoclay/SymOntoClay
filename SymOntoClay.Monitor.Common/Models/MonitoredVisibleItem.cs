using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Monitor.Common.Models
{
    public class MonitoredVisibleItem : IObjectToString
    {
        public string ObjectId { get; set; }
        public string PublicInformationHumanizedStr { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(ObjectId)} = {ObjectId}");
            sb.AppendLine($"{spaces}{nameof(PublicInformationHumanizedStr)} = {PublicInformationHumanizedStr}");

            return sb.ToString();
        }
    }
}
