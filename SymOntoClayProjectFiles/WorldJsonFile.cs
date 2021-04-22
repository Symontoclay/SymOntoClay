using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public class WorldJsonFile : IObjectToString
    {
        public string MainNpc { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(MainNpc)} = {MainNpc}");

            return sb.ToString();
        }

        public static WorldJsonFile LoadFromFile(string fileName)
        {
            return JsonConvert.DeserializeObject<WorldJsonFile>(File.ReadAllText(fileName));
        }
    }
}
