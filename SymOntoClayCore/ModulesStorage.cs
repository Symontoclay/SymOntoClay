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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymOntoClay.Core
{
    public class ModulesStorage : BaseComponent, IModulesStorage, ISerializableEngine
    {
        public ModulesStorage(ModulesStorageSettings settings)
            : base(settings.MonitorNode)
        {

            _libDirs = settings.LibsDirs;

            _hasLibDirs = !_libDirs.IsNullOrEmpty();
        }

        public void Init(IMainStorageContext mainStorageContext)
        {
            _projectLoader = new ProjectLoader(mainStorageContext, true);
            _mainStorageContext = mainStorageContext;
        }

        private ProjectLoader _projectLoader;
        private IMainStorageContext _mainStorageContext;

        private readonly object _lockObj = new object();

        private readonly IList<string> _libDirs;
        private readonly bool _hasLibDirs;

        private readonly Dictionary<StrongIdentifierValue, IStorage> _storagesDict = new Dictionary<StrongIdentifierValue, IStorage>();
        private readonly Dictionary<StrongIdentifierValue, List<StrongIdentifierValue>> _dependenciesDict = new Dictionary<StrongIdentifierValue, List<StrongIdentifierValue>>();

        /// <inheritdoc/>
        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }


#if IMAGINE_WORKING
#else
                throw new NotImplementedException("E224FE7C-FD9E-420D-AC77-6BF06A24ACB7");
#endif
            }
        }

        /// <inheritdoc/>
        public IList<IStorage> Import(IMonitorLogger logger, IList<StrongIdentifierValue> namesList)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }
            }

            lock(_lockObj)
            {
                var result = new List<IStorage>();

                if(!_hasLibDirs)
                {
                    return result;
                }

                var loadedLibNames = new List<string>();

                foreach (var name in namesList)
                {
                    NLoadLib(logger, name, result, loadedLibNames);
                }

                return result;
            }
        }

        private void NLoadLib(IMonitorLogger logger, StrongIdentifierValue name, List<IStorage> result, List<string> loadedLibNames)
        {
            var strName = name.NameValue;

            if(loadedLibNames.Contains(strName))
            {
                return;
            }

            loadedLibNames.Add(strName);

            if (_storagesDict.ContainsKey(name))
            {
                result.Add(_storagesDict[name]);

                if(_dependenciesDict.ContainsKey(name))
                {
                    var dependenciesList = _dependenciesDict[name];

                    foreach(var dependency in dependenciesList)
                    {
                        NLoadLib(logger, dependency, result, loadedLibNames);
                    }
                }
            }
            else
            {
                var libDirectory = GetLibDirectory(logger, strName);

                if (string.IsNullOrWhiteSpace(libDirectory))
                {
                    throw new FileNotFoundException($"Unable to find lib `{strName}`.");
                }

                var libFileName = GetLibFileName(logger, libDirectory);

                if (string.IsNullOrWhiteSpace(libFileName))
                {
                    throw new FileNotFoundException($"Unable to find lib `{strName}`.");
                }

                var storageSettings = new RealStorageSettings();
                storageSettings.MainStorageContext = _mainStorageContext;

                var storage = new LibStorage(storageSettings);

                var defferedLibsList = _projectLoader.LoadFromSourceFiles(logger, storage, libFileName).Select(p => NameHelper.CreateName(p)).ToList();

                _storagesDict[name] = storage;

                result.Add(storage);

                if (defferedLibsList.Any())
                {
                    _dependenciesDict[name] = defferedLibsList;

                    foreach (var defferedLib in defferedLibsList)
                    {
                        NLoadLib(logger, defferedLib, result, loadedLibNames);
                    }
                }                
            }
        }

        private string GetLibDirectory(IMonitorLogger logger, string name)
        {
            name = name.Trim().ToLower();

            foreach (var baseDirName in _libDirs)
            {
                var subDirectories = Directory.EnumerateDirectories(baseDirName);

                foreach(var subDirectory in subDirectories)
                {
                    var directoryInfo = new DirectoryInfo(subDirectory);

                    if(directoryInfo.Name.ToLower() == name)
                    {
                        return directoryInfo.FullName;
                    }
                }
            }

            return string.Empty;
        }

        private string GetLibFileName(IMonitorLogger logger, string directoryName)
        {
            var files = Directory.EnumerateFiles(directoryName).Where(p => p.EndsWith(".slib"));

            if(!files.Any())
            {
                return string.Empty;
            }

            if(files.Count() == 1)
            {
                return files.First();
            }

            throw new NotImplementedException("E497EC5B-DDDA-4B5E-AFDD-0C5A5B559BE4");
        }
    }
}
