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
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract class BaseStoredGameComponent: BaseGameComponent
    {
        protected BaseStoredGameComponent(BaseStoredGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            try
            {
                _idForFacts = settings.IdForFacts;

                var standaloneStorageSettings = new StandaloneStorageSettings();
                standaloneStorageSettings.Id = settings.Id;
                standaloneStorageSettings.IsWorld = false;
                standaloneStorageSettings.AppFile = settings.HostFile;
                standaloneStorageSettings.Logger = Logger;
                
                standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
                standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;
                standaloneStorageSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;

#if DEBUG
                //Log($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
                HostStorage = new StandaloneStorage(standaloneStorageSettings);
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }
        }

        private readonly string _idForFacts;

        protected StandaloneStorage HostStorage { get; private set; }

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => HostStorage.PublicFactsStorage;

        /// <inheritdoc/>
        public override string IdForFacts => _idForFacts;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
                HostStorage.LoadFromSourceCode();
            //}
            //catch (Exception e)
            //{
            //    Log(e.ToString());

            //    throw e;
            //}
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            HostStorage.Dispose();

            base.OnDisposed();
        }
    }
}
