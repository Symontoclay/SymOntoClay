using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.SerializationData
{
    public class ThreadLoggerSerializationData : IObjectToString
    {
        public IMonitorNode Parent { get; set; }
        public string ThreadId { get; set; }

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

            sb.PrintExisting(n, nameof(Parent), Parent);
            sb.AppendLine($"{spaces}{nameof(ThreadId)} = {ThreadId}");

            return sb.ToString();
        }
    }
}
