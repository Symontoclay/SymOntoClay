/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.StandardLibrary;
using SymOntoClay.Core.Internal.States;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.Core.Internal.Threads;

namespace SymOntoClay.Core.Internal
{
    public class EngineContext : MainStorageContext, IEngineContext
    {
        public EngineContext(IEntityLogger logger)
            : base(logger)
        {
        }

        public CodeExecutorComponent CodeExecutor { get; set; }
        public TriggerExecutorComponent TriggerExecutor { get; set; }
        public StatesStorageComponent StatesStorage { get; set; }       
        public LogicalEngineComponent LogicalEngine { get; set; }
        public InheritanceEngineComponent InheritanceEngine { get; set; }
        public StandardLibraryLoader StandardLibraryLoader { get; set; }       
        public ActivePeriodicObjectContext ActivePeriodicObjectContext { get; set; }
        public IHostSupport HostSupport { get; set; }
        public IHostListener HostListener { get; set; }

        ICodeExecutorComponent IEngineContext.CodeExecutor => CodeExecutor;
        ITriggerExecutorComponent IEngineContext.TriggerExecutor => TriggerExecutor;     
        ILogicalEngine IEngineContext.LogicalEngine => LogicalEngine;
        IInheritanceEngine IEngineContext.InheritanceEngine => InheritanceEngine;        
        IActivePeriodicObjectContext IEngineContext.ActivePeriodicObjectContext => ActivePeriodicObjectContext;
    }
}
