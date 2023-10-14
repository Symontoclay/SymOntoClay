using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class SpaceTextFileNameTemplateOptionItem : BaseFileNameTemplateOptionItem
    {
        /// <inheritdoc/>
        public override string ItemName { get => "SpaceText"; set => throw new NotImplementedException(); }

        public uint Widht { get; set; } = 1;

        /// <inheritdoc/>
        public override string GetText(string nodeId, string threadId)
        {
            if(IfNodeIdExists && string.IsNullOrWhiteSpace(nodeId))
            {
                return string.Empty;
            }

            if(IfThreadIdExists && string.IsNullOrWhiteSpace(threadId))
            {
                return string.Empty;
            }

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
