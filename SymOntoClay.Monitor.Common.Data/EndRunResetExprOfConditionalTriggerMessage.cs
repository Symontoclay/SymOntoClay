using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class EndRunResetExprOfConditionalTriggerMessage : BaseEndRunExprOfConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.EndRunResetExprOfConditionalTrigger;
    }
}
