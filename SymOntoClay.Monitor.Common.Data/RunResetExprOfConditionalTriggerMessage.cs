using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class RunResetExprOfConditionalTriggerMessage: BaseRunExprOfConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.RunResetExprOfConditionalTrigger;
    }
}
