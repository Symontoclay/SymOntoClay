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
        public DateTimeStampTextRowOptionItem(string format)
        {
            _format = format;
        }

        private readonly string _format;

        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message)
        {
            return message.DateTimeStamp.ToString(_format);
        }
    }
}
