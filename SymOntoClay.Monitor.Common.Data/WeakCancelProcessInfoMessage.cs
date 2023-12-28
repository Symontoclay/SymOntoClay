using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class WeakCancelProcessInfoMessage : BaseCancelMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.WeakCancelProcessInfo;
    }
}
