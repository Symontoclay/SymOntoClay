/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Compiling;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.CoreHelper.DebugHelpers;

namespace SymOntoClay.Core.Internal
{
    public interface IMainStorageContext: IBaseCoreContext
    {
        string Id { get; }
        string AppFile { get; }        
        
        IStorageComponent Storage { get; }
        IParser Parser { get; }
        ICompiler Compiler { get; }
        IDataResolversFactory DataResolversFactory { get; }
        ICommonNamesStorage CommonNamesStorage { get; }
        ILoaderFromSourceCode LoaderFromSourceCode { get; }
        ILogicQueryParseAndCache LogicQueryParseAndCache { get; }
    }
}
