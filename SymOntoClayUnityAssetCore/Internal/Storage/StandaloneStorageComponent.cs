/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Storage
{
    public class StandaloneStorageComponent: BaseWorldCoreComponent
    {
        public StandaloneStorageComponent(WorldSettings settings, IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = "world";
            standaloneStorageSettings.IsWorld = true;
            standaloneStorageSettings.Logger = coreContext.Logger;
            standaloneStorageSettings.ModulesStorage = coreContext.ModulesStorage;
            standaloneStorageSettings.Dictionary = coreContext.SharedDictionary;
            standaloneStorageSettings.AppFile = settings.HostFile;
            standaloneStorageSettings.LogicQueryParseAndCache = coreContext.LogicQueryParseAndCache;

            //Log($"standaloneStorageSettings = {standaloneStorageSettings}");

            _standaloneStorage = new StandaloneStorage(standaloneStorageSettings);
        }

        private readonly StandaloneStorage _standaloneStorage;

        public IStandaloneStorage StandaloneStorage => _standaloneStorage;

        public void LoadFromSourceCode()
        {
            _standaloneStorage.LoadFromSourceCode();
        }
    }
}
