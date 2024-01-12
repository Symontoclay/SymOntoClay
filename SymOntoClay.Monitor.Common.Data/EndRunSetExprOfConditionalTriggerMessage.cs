using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class EndRunSetExprOfConditionalTriggerMessage: BaseEndRunExprOfConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.EndRunSetExprOfConditionalTrigger;
    }
}
