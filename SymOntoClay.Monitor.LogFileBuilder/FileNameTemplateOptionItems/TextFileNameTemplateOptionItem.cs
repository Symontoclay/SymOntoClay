using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class TextFileNameTemplateOptionItem : BaseFileNameTemplateOptionItem
    {
        public TextFileNameTemplateOptionItem() 
        {
        }

        public TextFileNameTemplateOptionItem(string text)
        {
            Text = text;
        }

        /// <inheritdoc/>
        public override string ItemName { get => "Text"; set => throw new NotImplementedException(); }

        public string Text {  get; set; }

        /// <inheritdoc/>
        public override string GetText(string nodeId, string threadId)
        {
            if(string.IsNullOrWhiteSpace(Text))
            {
                return string.Empty;
            }

            if (IfNodeIdExists && string.IsNullOrWhiteSpace(nodeId))
            {
                return string.Empty;
            }

            if (IfThreadIdExists && string.IsNullOrWhiteSpace(threadId))
            {
                return string.Empty;
            }

            return Text;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Text)} = {Text}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
