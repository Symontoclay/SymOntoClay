using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class NLPHandler
    {
        public NLPHandler()
        {
            var context = TstEngineContextHelper.CreateAndInitContext();

            _engineContext = context.EngineContext;
            _worldContext = context.WorldContext;
        }

        private readonly EngineContext _engineContext;
        private readonly WorldContext _worldContext;
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }
    }
}
