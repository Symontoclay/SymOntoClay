/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Tests.Helpers
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
