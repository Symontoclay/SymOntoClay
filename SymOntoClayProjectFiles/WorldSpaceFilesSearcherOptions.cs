using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public class WorldSpaceFilesSearcherOptions : IObjectToString
    {
        public string InputFile { get; set; }
        public string InputDir { get; set; }
        public string AppName { get; set; }
        public string BaseTempDir { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(InputFile)} = {InputFile}");
            sb.AppendLine($"{spaces}{nameof(InputDir)} = {InputDir}");
            sb.AppendLine($"{spaces}{nameof(AppName)} = {AppName}");
            sb.AppendLine($"{spaces}{nameof(BaseTempDir)} = {BaseTempDir}");

            return sb.ToString();
        }
    }
}
