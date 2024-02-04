using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class SpaceTextRowOptionItem : BaseMessageTextRowOptionItem
    {
        public uint Widht { get; set; } = 1;

        public SpaceTextRowOptionItem()
        {
        }

        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return DisplayHelper.Spaces(Widht);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Widht)} = {Widht}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
