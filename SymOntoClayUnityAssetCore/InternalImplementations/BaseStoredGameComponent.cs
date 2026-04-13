/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract class BaseStoredGameComponent: BaseGameComponent
    {
        protected BaseStoredGameComponent(BaseStoredGameComponentSettings settings, IWorldCoreGameComponentContext worldContext, KindOfWorldItem kindOfWorldItem)
            : base(settings, worldContext, kindOfWorldItem)
        {
            try
            {
                _settings = settings;
                _kindOfWorldItem = kindOfWorldItem;

                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, worldContext);
                _soundPublisher = new SoundPublisherComponent(Logger, settings.InstanceId, settings.IdForFacts, _hostSupport, worldContext);
            }
            catch (Exception e)
            {
                Error("FF3FAC3F-DEDE-4556-A475-CE7EEF5E9902", e);

                throw e;
            }
        }

        private readonly BaseStoredGameComponentSettings _settings;
        private readonly KindOfWorldItem _kindOfWorldItem;

        private BaseStoredGameComponentSerializedContext _internalSerializedContext;

        private readonly HostSupportComponent _hostSupport;
        private readonly SoundPublisherComponent _soundPublisher;

        protected StandaloneStorage HostStorage => _internalSerializedContext.HostStorage;

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => _internalSerializedContext.HostStorage.PublicFactsStorage;

        private void CreateSerializedComponents()
        {
            _internalSerializedContext?.Dispose();
            _internalSerializedContext = new BaseStoredGameComponentSerializedContext(_settings, _worldContext, _kindOfWorldItem, this);
        }

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            _internalSerializedContext?.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);
        }

        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _internalSerializedContext.HostStorage.InsertPublicFact(logger, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _internalSerializedContext.HostStorage.InsertPublicFact(logger, fact);
        }

        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            _internalSerializedContext.HostStorage.RemovePublicFact(logger, id);
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
            _internalSerializedContext.HostStorage.AddCategory(logger, category);
        }

        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalSerializedContext.HostStorage.AddCategories(logger, categories);
        }

        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            _internalSerializedContext.HostStorage.RemoveCategory(logger, category);
        }

        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _internalSerializedContext.HostStorage.RemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _internalSerializedContext.HostStorage.EnableCategories; set => _internalSerializedContext.HostStorage.EnableCategories = value; }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _internalSerializedContext?.Dispose();

            base.OnDisposed();
        }
    }
}
