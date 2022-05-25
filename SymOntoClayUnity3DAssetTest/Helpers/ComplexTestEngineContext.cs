using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public class ComplexTestEngineContext: ISymOntoClayDisposable
    {
        public ComplexTestEngineContext(WorldContext worldContext, EngineContext engineContext, string testDir)
        {
            WorldContext = worldContext;
            EngineContext = engineContext;
            _testDir = testDir;
        }

        public EngineContext EngineContext { get; private set; }
        public WorldContext WorldContext { get; private set; }
        private readonly string _testDir;

        public void Start()
        {
            WorldContext.Start();
        }

        /// <inheritdoc/>
        public bool IsDisposed => WorldContext.IsDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            WorldContext.Dispose();

            if(!string.IsNullOrWhiteSpace(_testDir) && Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }
    }
}
