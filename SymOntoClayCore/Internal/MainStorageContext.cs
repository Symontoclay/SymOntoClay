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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Services;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public class MainStorageContext: BaseCoreContext, IMainStorageContext
    {
        public MainStorageContext(IMonitorNode monitorNode)
            : base(monitorNode)
        {
        }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public StrongIdentifierValue SelfName { get; set; }

        /// <inheritdoc/>
        public string AppFile { get; set; }

        public ActiveObjectContext ActiveObjectContext { get; set; }

        public StorageComponent Storage { get; set; }
        public Parser Parser { get; set; }
        public DataResolversFactory DataResolversFactory { get; set; }
        public ConvertersFactory ConvertersFactory { get; set; }
        public CommonNamesStorage CommonNamesStorage { get; set; }
        public BaseInstancesStorageComponent InstancesStorage { get; set; }
        public BaseLoaderFromSourceCode LoaderFromSourceCode { get; set; }

        public ServicesFactory ServicesFactory { get; set; }

        /// <inheritdoc/>
        public ILogicQueryParseAndCache LogicQueryParseAndCache { get; set; }

        /// <inheritdoc/>
        public IModulesStorage ModulesStorage { get; set; }

        IStorageComponent IMainStorageContext.Storage => Storage;
        IParser IMainStorageContext.Parser => Parser;
        
        IDataResolversFactory IMainStorageContext.DataResolversFactory => DataResolversFactory;
        IConvertersFactory IMainStorageContext.ConvertersFactory => ConvertersFactory;
        ICommonNamesStorage IMainStorageContext.CommonNamesStorage => CommonNamesStorage;
        ILoaderFromSourceCode IMainStorageContext.LoaderFromSourceCode => LoaderFromSourceCode;
        IInstancesStorageComponent IMainStorageContext.InstancesStorage => InstancesStorage;

        IActiveObjectContext IMainStorageContext.ActivePeriodicObjectContext => ActiveObjectContext;

        IServicesFactory IMainStorageContext.ServicesFactory => ServicesFactory;

        public virtual void Die()
        {
            ActiveObjectContext.Dispose();
            Storage.Die();
            Parser.Dispose();
            CommonNamesStorage.Dispose();
            InstancesStorage.Dispose();
            LoaderFromSourceCode.Dispose();
            ServicesFactory.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            ActiveObjectContext.Dispose();
            Storage.Dispose();
            Parser.Dispose();
            CommonNamesStorage.Dispose();
            InstancesStorage.Dispose();
            LoaderFromSourceCode.Dispose();
            ServicesFactory.Dispose();

            base.OnDisposed();
        }
    }
}
