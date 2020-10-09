/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IEngineContext: IMainStorageContext
    {   
        ICodeExecutorComponent CodeExecutor { get; }
        ITriggerExecutorComponent TriggerExecutor { get; }
        ILogicalEngine LogicalEngine { get; }
        IInheritanceEngine InheritanceEngine { get; }
        IInstancesStorageComponent InstancesStorage { get; }
        IActivePeriodicObjectContext ActivePeriodicObjectContext { get; }
        IHostSupport HostSupport { get; }
        IHostListener HostListener { get; }
    }
}
