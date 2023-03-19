/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
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
                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, worldContext);
                _soundPublisher = new SoundPublisherComponent(Logger, settings.InstanceId, settings.IdForFacts, _hostSupport, worldContext);

                var standaloneStorageSettings = new StandaloneStorageSettings();
                standaloneStorageSettings.Id = settings.Id;
                standaloneStorageSettings.IsWorld = false;
                standaloneStorageSettings.AppFile = settings.HostFile;
                standaloneStorageSettings.Logger = Logger;
                
                standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
                standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;
                standaloneStorageSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;
                standaloneStorageSettings.KindOfLogicalSearchExplain = worldContext.KindOfLogicalSearchExplain;
                standaloneStorageSettings.LogicalSearchExplainDumpDir = worldContext.LogicalSearchExplainDumpDir;
                standaloneStorageSettings.EnableAddingRemovingFactLoggingInStorages = worldContext.EnableAddingRemovingFactLoggingInStorages;

                standaloneStorageSettings.Categories = settings.Categories;
                standaloneStorageSettings.EnableCategories = settings.EnableCategories;

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
        
        private readonly HostSupportComponent _hostSupport;
        private readonly SoundPublisherComponent _soundPublisher;

        protected StandaloneStorage HostStorage { get; private set; }

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => HostStorage.PublicFactsStorage;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
                HostStorage.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);
            //}
            //catch (Exception e)
            //{
            //    Log(e.ToString());

            //    throw e;
            //}
        }

        public string InsertPublicFact(string text)
        {
            return HostStorage.InsertPublicFact(text);
        }

        public string InsertPublicFact(RuleInstance fact)
        {
            return HostStorage.InsertPublicFact(fact);
        }

        public void RemovePublicFact(string id)
        {
            HostStorage.RemovePublicFact(id);
        }

        public void PushSoundFact(float power, string text)
        {
            _soundPublisher.PushSoundFact(power, text);
        }

        public void PushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.PushSoundFact(power, fact);
        }

        public void AddCategory(string category)
        {
            HostStorage.AddCategory(category);
        }

        public void AddCategories(List<string> categories)
        {
            HostStorage.AddCategories(categories);
        }

        public void RemoveCategory(string category)
        {
            HostStorage.RemoveCategory(category);
        }

        public void RemoveCategories(List<string> categories)
        {
            HostStorage.RemoveCategories(categories);
        }

        public bool EnableCategories { get => HostStorage.EnableCategories; set => HostStorage.EnableCategories = value; }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            HostStorage.Dispose();

            base.OnDisposed();
        }
    }
}
