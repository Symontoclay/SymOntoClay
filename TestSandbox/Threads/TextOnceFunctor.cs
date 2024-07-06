using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.Threads
{
    public class TextOnceFunctor
    {
        public TextOnceFunctor(string text, IMonitorLogger logger, IActiveObjectContext context, ICustomThreadPool threadPool)
        {
            _text = text;
            _logger = logger;
            _asyncActiveOnceObject = new AsyncActiveOnceObject(context, threadPool, logger)
            {
                OnceMethod = OnceMethod
            };
        }

        private string _text;
        private readonly AsyncActiveOnceObject _asyncActiveOnceObject;
        private readonly IMonitorLogger _logger;

        public Value Start()
        {
            return _asyncActiveOnceObject.Start();
        }

        private void OnceMethod(CancellationToken cancellationToken)
        {
            _logger.Info("8694EC22-C2EB-464A-84FA-1BC65AE0ADCE", $"_text = {_text}");
        }
    }
}
