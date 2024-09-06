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
using SymOntoClay.ActiveObject.MethodResponses;
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

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("1D4368C5-491C-4CA9-BAEA-DDF09127F444");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return HostStorage.InsertPublicFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotSupportedException("CC1F1DE6-AA4D-4DB1-9410-7A57F3C61A0D");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return HostStorage.InsertPublicFact(logger, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotSupportedException("532D67F0-6213-4D23-97AF-0D819573576E");
        }

        /// <inheritdoc/>
        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return HostStorage.RemovePublicFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, string text)
        {
            throw new NotSupportedException("0590467C-6827-42CD-8DEF-0373A0FA53ED");
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, string text)
        {
            return _soundPublisher.PushSoundFact(power, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, RuleInstance fact)
        {
            throw new NotSupportedException("DD329FFB-3954-4652-B4CF-F4DE6BBF24B5");
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            return _soundPublisher.PushSoundFact(power, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("40EFC9DE-246D-47E6-A0AE-BF69A99967A9");
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return HostStorage.AddCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("A98D1117-9DA9-4AFF-926D-64A7C346EF9D");
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

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("C05CFA63-21D9-4FBC-8741-75FBE8C2D09E");
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return HostStorage.RemoveCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("D0FD4FCD-C0E1-470E-9C96-6B42F7B453BD");
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
