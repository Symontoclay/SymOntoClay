using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CheckDirtyOptions: IObjectToString
    {
        public IEngineContext EngineContext { get; set; }
        public LocalCodeExecutionContext LocalContext { get; set; }
        public bool ConvertWaypointValueFromSource { get; set; }

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

            sb.PrintExisting(n, nameof(EngineContext), EngineContext);
            sb.PrintExisting(n, nameof(LocalContext), LocalContext);
            sb.AppendLine($"{spaces}{nameof(ConvertWaypointValueFromSource)} = {ConvertWaypointValueFromSource}");

            return sb.ToString();
        }
    }
}
