using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class SetConditionalTriggerMessage : BaseConditionalTriggerMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.SetConditionalTrigger;
    }
}
