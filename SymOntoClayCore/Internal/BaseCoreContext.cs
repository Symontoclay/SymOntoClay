/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal
{
    public class BaseCoreContext: BaseComponent, IBaseCoreContext
    {
        public BaseCoreContext(IMonitorNode monitorNode)
            : base(monitorNode)
        {
            MonitorNode = monitorNode;
        }

        /// <inheritdoc/>
        public IMonitorNode MonitorNode { get; private set; }

        public Compiler Compiler { get; set; }

        /// <inheritdoc/>
        ICompiler IBaseCoreContext.Compiler => Compiler;

        /// <inheritdoc/>
        public IDateTimeProvider DateTimeProvider { get; set; }

        /// <inheritdoc/>
        public IStandardCoreFactsBuilder StandardFactsBuilder { get; set; }

        /// <inheritdoc/>
        public ICustomThreadPool AsyncEventsThreadPool { get; set; }

        /// <inheritdoc/>
        public ICustomThreadPool GarbageCollectionThreadPool { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationTokenSource LinkedCancellationTokenSource { get; set; }

        /// <inheritdoc/>
        public CancellationToken GetCancellationToken()
        {
            return LinkedCancellationTokenSource.Token;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            CancellationTokenSource.Dispose();

            AsyncEventsThreadPool.Dispose();
            GarbageCollectionThreadPool.Dispose();

            base.OnDisposed();
        }
    }
}
