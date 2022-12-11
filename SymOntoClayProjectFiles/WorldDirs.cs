using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ProjectFiles
{
    public class WorldDirs : IObjectToString
    {
        public string Libs { get; set; }
        public string Navs { get; set; }
        public string Npcs { get; set; }
        public string Players { get; set; }
        public string SharedLibs { get; set; }
        public string Things { get; set; }
        public string World { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Libs)} = {Libs}"); 
            sb.AppendLine($"{spaces}{nameof(Navs)} = {Navs}");
            sb.AppendLine($"{spaces}{nameof(Npcs)} = {Npcs}");
            sb.AppendLine($"{spaces}{nameof(Players)} = {Players}");
            sb.AppendLine($"{spaces}{nameof(SharedLibs)} = {SharedLibs}");
            sb.AppendLine($"{spaces}{nameof(Things)} = {Things}");
            sb.AppendLine($"{spaces}{nameof(World)} = {World}");

            return sb.ToString();
        }
    }
}
