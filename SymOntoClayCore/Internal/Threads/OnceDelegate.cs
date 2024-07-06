using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public delegate void OnceDelegate(CancellationToken cancellationToken);
}
