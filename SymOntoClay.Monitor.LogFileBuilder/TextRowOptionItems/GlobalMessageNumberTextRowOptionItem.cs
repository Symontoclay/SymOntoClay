using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class GlobalMessageNumberTextRowOptionItem : BaseMessageTextRowOptionItem
    {
        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return message.GlobalMessageNumber.ToString();
        }
    }
}
