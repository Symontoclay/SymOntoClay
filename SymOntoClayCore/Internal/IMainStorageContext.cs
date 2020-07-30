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
    public interface IMainStorageContext
    {
        string Id { get; }
        string AppFile { get; }

        IEntityLogger Logger { get; }
        IEntityDictionary Dictionary { get; }
        IStorageComponent Storage { get; }
        IParser Parser { get; }
        ICompiler Compiler { get; }
        IDataResolversFactory DataResolversFactory { get; }
        ICommonNamesStorage CommonNamesStorage { get; }
        ILoaderFromSourceCode LoaderFromSourceCode { get; }
    }
}
