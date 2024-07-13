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

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class AsyncThreadExecutor: BaseThreadExecutor
    {
        public AsyncThreadExecutor(IEngineContext context, ICustomThreadPool threadPool)
            : base(context, new AsyncActivePeriodicObject(context.ActiveObjectContext, threadPool, context.Logger), BaseThreadExecutor.CreateInitParams(context))
        {
        }

        public AsyncThreadExecutor(IEngineContext context, ICustomThreadPool threadPool, string parentThreadId)
            : base(context, new AsyncActivePeriodicObject(context.ActiveObjectContext, threadPool, context.Logger), BaseThreadExecutor.CreateInitParams(context, parentThreadId))
        {
        }
        
        public AsyncThreadExecutor(IEngineContext context, ICustomThreadPool threadPool, IMonitorLogger logger)
            : this(context, threadPool, logger, logger.Id)
        {
        }

        public AsyncThreadExecutor(IEngineContext context, ICustomThreadPool threadPool, IMonitorLogger logger, string threadId)
            : base(context, new AsyncActivePeriodicObject(context.ActiveObjectContext, threadPool, logger), logger, threadId)
        {
        }
    }
}
