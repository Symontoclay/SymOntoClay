using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class SystemExprMessage : BaseLabeledValueMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.SystemExpr;
    }
}
