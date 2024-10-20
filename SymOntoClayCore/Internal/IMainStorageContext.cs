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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Compiling;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Core.Internal.Converters;

namespace SymOntoClay.Core.Internal
{
    public interface IMainStorageContext: IBaseCoreContext
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        string Id { get; }
        StrongIdentifierValue SelfName { get; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        string AppFile { get; }

        IActivePeriodicObjectContext ActivePeriodicObjectContext { get; }

        IStorageComponent Storage { get; }
        IParser Parser { get; }
        IDataResolversFactory DataResolversFactory { get; }
        IConvertersFactory ConvertersFactory { get; }
        ICommonNamesStorage CommonNamesStorage { get; }
        ILoaderFromSourceCode LoaderFromSourceCode { get; }
        ILogicQueryParseAndCache LogicQueryParseAndCache { get; }
        IInstancesStorageComponent InstancesStorage { get; }
        IModulesStorage ModulesStorage { get; }

        IServicesFactory ServicesFactory { get; }
    }
}
