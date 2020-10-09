/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        public InstancesStorageComponent InstancesStorage { get; set; }
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
        IInstancesStorageComponent IEngineContext.InstancesStorage => InstancesStorage;
        IActivePeriodicObjectContext IEngineContext.ActivePeriodicObjectContext => ActivePeriodicObjectContext;
    }
}
