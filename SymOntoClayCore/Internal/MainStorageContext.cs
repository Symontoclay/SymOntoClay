using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public class MainStorageContext: BaseComponent, IMainStorageContext
    {
        public MainStorageContext(IEntityLogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        public IEntityDictionary Dictionary { get; set; }

        public StorageComponent Storage { get; set; }
        public Parser Parser { get; set; }
        public Compiler Compiler { get; set; }
        public DataResolversFactory DataResolversFactory { get; set; }
        public CommonNamesStorage CommonNamesStorage { get; set; }
        public BaseLoaderFromSourceCode LoaderFromSourceCode { get; set; }

        IStorageComponent IMainStorageContext.Storage => Storage;
        IParser IMainStorageContext.Parser => Parser;
        ICompiler IMainStorageContext.Compiler => Compiler;
        IDataResolversFactory IMainStorageContext.DataResolversFactory => DataResolversFactory;
        ICommonNamesStorage IMainStorageContext.CommonNamesStorage => CommonNamesStorage;
        ILoaderFromSourceCode IMainStorageContext.LoaderFromSourceCode => LoaderFromSourceCode;
    }
}
