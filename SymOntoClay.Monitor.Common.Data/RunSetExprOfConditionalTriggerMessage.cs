using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class RunSetExprOfConditionalTriggerMessage: BaseRunExprOfConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.RunSetExprOfConditionalTrigger;
    }
}
