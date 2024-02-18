using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class RemoveFactFromLogicalStorageMessage : BaseFactLogicalStorageMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.RemoveFactFromLogicalStorage;
    }
}
