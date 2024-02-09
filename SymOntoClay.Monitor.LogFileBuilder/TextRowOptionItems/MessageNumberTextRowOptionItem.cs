using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class MessageNumberTextRowOptionItem : BaseMessageTextRowOptionItem
    {
        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext, string targetFileName)
        {
            return message.MessageNumber.ToString();
        }
    }
}
