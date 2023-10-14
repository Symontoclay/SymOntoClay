using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FileNameTemplateOptionItems
{
    public class DateTimeStampFileNameTemplateOptionItem : BaseFileNameTemplateOptionItem
    {
        public DateTimeStampFileNameTemplateOptionItem()
        {
        }

        public DateTimeStampFileNameTemplateOptionItem(string format)
        {
            Format = format;
        }

        /// <inheritdoc/>
        public override string ItemName { get => "DateTimeStamp"; set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public override string GetText(string nodeId, string threadId)
        {
            if (IfNodeIdExists && string.IsNullOrWhiteSpace(nodeId))
            {
                return string.Empty;
            }

            if (IfThreadIdExists && string.IsNullOrWhiteSpace(threadId))
            {
                return string.Empty;
            }

            return DateTime.Now.ToString(Format);
        }
    }
}
