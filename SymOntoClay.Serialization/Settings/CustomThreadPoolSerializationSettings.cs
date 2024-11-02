using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization.SmartValues;
using System.Text;
using System.Threading;

namespace SymOntoClay.Serialization.Settings
{
    public class CustomThreadPoolSerializationSettings : IObjectToString
    {
        public SmartValue<int?> MinThreadsCount { get; set; }
        public SmartValue<int?> MaxThreadsCount { get; set; }
        public CancellationToken? CancellationToken { get; set; }

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
            sb.PrintExisting(n, nameof(CancellationToken), CancellationToken);
            sb.PrintObjProp(n, nameof(MinThreadsCount), MinThreadsCount);
            sb.PrintObjProp(n, nameof(MaxThreadsCount), MaxThreadsCount);
            return sb.ToString();
        }
    }
}
