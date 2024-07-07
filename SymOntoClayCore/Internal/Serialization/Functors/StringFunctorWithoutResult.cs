using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.Core.Internal.Serialization.Functors
{
    public class StringFunctorWithoutResult: BaseFunctorWithoutResult<IMonitorLogger, string>
    {
        public static void Run(IMonitorLogger logger, string text, Action<IMonitorLogger, string> action, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            var functor = new StringFunctorWithoutResult(logger, text, action, context, threadPool);
        }

        public StringFunctorWithoutResult(IMonitorLogger logger, string text, Action<IMonitorLogger, string> action, IActiveObjectContext context, ICustomThreadPool threadPool)
            : base(logger, logger, text, action, context, threadPool)
        {
        }
    }
}
