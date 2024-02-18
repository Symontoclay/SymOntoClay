using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common.Data
{
    public class PutFactForRemovingFromLogicalStorageMessage : BaseFactLogicalStorageMessage
    {
        /// <inheritdoc/>
        public override KindOfMessage KindOfMessage => KindOfMessage.PutFactForRemovingFromLogicalStorage;
    }
}
