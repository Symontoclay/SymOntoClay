using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class MessageContentTextRowOptionItem : BaseMessageTextRowOptionItem
    {
        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message)
        {
            return MessageContentToTextConverter.GetText(message);
        }
    }
}
