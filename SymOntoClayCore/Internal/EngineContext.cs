/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.StandardLibrary;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;

namespace SymOntoClay.Core.Internal
{
    public class EngineContext : MainStorageContext, IEngineContext
    {
        public EngineContext(IEntityLogger logger)
            : base(logger)
        {
        }

        public CodeExecutorComponent CodeExecutor { get; set; }       
        public StandardLibraryLoader StandardLibraryLoader { get; set; }       
        
        public IHostSupport HostSupport { get; set; }
        public IHostListener HostListener { get; set; }
        public IConditionalEntityHostSupport ConditionalEntityHostSupport { get; set; }

        ICodeExecutorComponent IEngineContext.CodeExecutor => CodeExecutor;

        /// <inheritdoc/>
        public override void Die()
        {
            CodeExecutor.Dispose();
            
            base.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            CodeExecutor.Dispose();
            
            base.OnDisposed();
        }
    }
}
