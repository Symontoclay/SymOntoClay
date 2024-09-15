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
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
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
                _directHostStorage = HostStorage;

                _activeObjectContext = ActiveObjectContext;
                _serializationAnchor = new SerializationAnchor();
            }
            catch (Exception e)
            {
                Error("FF3FAC3F-DEDE-4556-A475-CE7EEF5E9902", e);

                throw e;
            }
        }
        
        private HostSupportComponent _hostSupport;
        private SoundPublisherComponent _soundPublisher;

        private IActiveObjectContext _activeObjectContext;
        private SerializationAnchor _serializationAnchor;

        protected StandaloneStorage HostStorage { get; private set; }

        private IDirectStandaloneStorage _directHostStorage;

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => HostStorage.PublicFactsStorage;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            HostStorage.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return HostStorage.InsertPublicFact(logger, text);
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return HostStorage.InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return HostStorage.RemovePublicFact(logger, id);
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, string text)
        {
            return LoggedSyncFunctorWithoutResult<BaseStoredGameComponent, float, string>.Run(Logger, "06AB8E90-47E0-42DD-830E-CEB5B298FEF6", this, power, text,
                (IMonitorLogger loggerValue, BaseStoredGameComponent instanceValue, float powerValue, string textValue) => {
                    instanceValue.NPushSoundFact(powerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NPushSoundFact(float power, string text)
        {
            _soundPublisher.PushSoundFact(power, text);
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            return LoggedSyncFunctorWithoutResult<BaseStoredGameComponent, float, RuleInstance>.Run(Logger, "9B6625A9-8F6B-479D-9541-90638BB7BA68", this, power, fact,
                (IMonitorLogger loggerValue, BaseStoredGameComponent instanceValue, float powerValue, RuleInstance factValue) => {
                    instanceValue.NPushSoundFact(powerValue, factValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NPushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.PushSoundFact(power, fact);
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return HostStorage.AddCategory(logger, category);
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return HostStorage.AddCategories(logger, categories);
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _directHostStorage.DirectAddCategories(logger, categories);
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return HostStorage.RemoveCategory(logger, category);
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return HostStorage.RemoveCategories(logger, categories);
        }

        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _directHostStorage.DirectRemoveCategories(logger, categories);
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
