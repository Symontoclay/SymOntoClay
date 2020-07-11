using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Compiling;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IMainStorageContext: IParserContext
    {
        IParser Parser { get; }
        ICompiler Compiler { get; }
        IDataResolversFactory DataResolversFactory { get; }
        ICommonNamesStorage CommonNamesStorage { get; }
    }
}
