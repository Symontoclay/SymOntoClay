using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public abstract class BaseFileNameTemplateOptionItem : IObjectToString
    {
        public abstract string GetText(string nodeId, string threadId);

        public bool IfNodeIdExists { get; set; }
        public bool IfThreadIdExists { get; set; }

        public virtual bool IsExtension => false;

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
            sb.AppendLine($"{spaces}{nameof(IfNodeIdExists)} = {IfNodeIdExists}");
            sb.AppendLine($"{spaces}{nameof(IfThreadIdExists)} = {IfThreadIdExists}");
            sb.AppendLine($"{spaces}{nameof(IsExtension)} = {IsExtension}");
            return sb.ToString();
        }
    }
}
