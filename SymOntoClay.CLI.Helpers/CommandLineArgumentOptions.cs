using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI.Helpers
{
    public class CommandLineArgumentOptions : IObjectToString
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public List<string> Names => new List<string> { Name }.Concat(Aliases ?? new List<string>()).ToList();
        public KindOfCommandLineArgument Kind { get; set; }
        public bool IsUnique { get; set; }
        public bool IsDefault { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.PrintPODList(n, nameof(Aliases), Aliases);
            sb.PrintPODList(n, nameof(Names), Names);
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsUnique)} = {IsUnique}");
            sb.AppendLine($"{spaces}{nameof(IsDefault)} = {IsDefault}");
            return sb.ToString();
        }
    }
}
