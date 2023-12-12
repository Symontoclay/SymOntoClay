using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.TextRowOptionItems
{
    public class MessageContentTextRowOptionItem : BaseMessageTextRowOptionItem, IMessageContentToTextConverterOptions
    {
        public MessageContentTextRowOptionItem() 
        {
            _messageContentToTextConverter = new MessageContentToTextConverter(this);
        }

        private readonly MessageContentToTextConverter _messageContentToTextConverter;

        /// <inheritdoc/>
        protected override string GetContent(BaseMessage message)
        {
            return _messageContentToTextConverter.GetText(message);
        }
    }
}
