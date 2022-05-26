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
        public ComplexTestEngineContext(IWorld world, IHumanoidNPC humanoidNPC)
            : this(world, humanoidNPC, string.Empty)
        {
        }

        public ComplexTestEngineContext(IWorld world, IHumanoidNPC humanoidNPC, string baseDir)
        {
            World = world;
            HumanoidNPC = humanoidNPC;
            _baseDir = baseDir;
        }

        public IEngineContext EngineContext => HumanoidNPC.EngineContext;
        public IWorld World { get; private set; }
        public WorldContext WorldContext => World.WorldContext;
        public IHumanoidNPC HumanoidNPC { get; private set; }
        private readonly string _baseDir;

        public void Start()
        {
            World.Start();
        }

        /// <inheritdoc/>
        public bool IsDisposed => World.IsDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            World.Dispose();

            if(!string.IsNullOrWhiteSpace(_baseDir) && Directory.Exists(_baseDir))
            {
                Directory.Delete(_baseDir, true);
            }
        }
    }
}
