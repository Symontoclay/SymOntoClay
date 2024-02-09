using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class DateTimeStampTextRowOptionItem : BaseMessageTextRowOptionItem
    {
        public DateTimeStampTextRowOptionItem()
        {
        }

        public DateTimeStampTextRowOptionItem(string format)
        {
            Format = format;
        }

        public string Format { get; set; }

        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext, string targetFileName)
        {
            return message.DateTimeStamp.ToString(Format);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Format)} = {Format}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
