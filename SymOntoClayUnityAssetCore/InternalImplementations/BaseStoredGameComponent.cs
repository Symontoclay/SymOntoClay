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

using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract partial class BaseStoredGameComponent : BaseGameComponent
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
                standaloneStorageSettings.MonitorNode = MonitorNode;
                
                standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
                standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;
                standaloneStorageSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;

                standaloneStorageSettings.Categories = settings.Categories;
                standaloneStorageSettings.EnableCategories = settings.EnableCategories;

                standaloneStorageSettings.ThreadingSettings = settings?.ThreadingSettings ?? worldContext.ThreadingSettings;
                standaloneStorageSettings.CancellationToken = worldContext.GetCancellationToken();

                HostStorage = new StandaloneStorage(standaloneStorageSettings);              
            }
            catch (Exception e)
            {
                Error("FF3FAC3F-DEDE-4556-A475-CE7EEF5E9902", e);

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

            HostStorage.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            return HostStorage.OldInsertPublicFact(logger, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return HostStorage.InsertPublicFact(logger, fact);
        }

        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            HostStorage.RemovePublicFact(logger, id);
        }

        public void PushSoundFact(float power, string text)
        {
            _soundPublisher.PushSoundFact(power, text);
        }

        public void PushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.PushSoundFact(power, fact);
        }

        public void AddCategory(IMonitorLogger logger, string category)
        {
            HostStorage.AddCategory(logger, category);
        }

        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            HostStorage.AddCategories(logger, categories);
        }

        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            HostStorage.RemoveCategory(logger, category);
        }

        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            HostStorage.RemoveCategories(logger, categories);
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
