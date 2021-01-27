/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageContext
    {
        public IMainStorageContext MainStorageContext { get; set; }
        public LogicalStorage.LogicalStorage LogicalStorage { get; set; }
        public MethodsStorage.MethodsStorage MethodsStorage { get; set; }
        public TriggersStorage.TriggersStorage TriggersStorage { get; set; }
        public InheritanceStorage.InheritanceStorage InheritanceStorage { get; set; }
        public SynonymsStorage.SynonymsStorage SynonymsStorage { get; set; }
        public OperatorsStorage.OperatorsStorage OperatorsStorage { get; set; }
        public ChannelsStorage.ChannelsStorage ChannelsStorage { get; set; }
        public MetadataStorage.MetadataStorage MetadataStorage { get; set; }
        public VarStorage.VarStorage VarStorage { get; set; }
        public RealStorage Storage { get; set; }
        public IList<IStorage> Parents { get; set; }
        public IInheritancePublicFactsReplicator InheritancePublicFactsReplicator { get; set; }
    }
}
