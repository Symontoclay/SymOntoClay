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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC
{
    public class HumanoidNPCGameComponent: BaseManualControllingGameComponent
    {
        public HumanoidNPCGameComponent(HumanoidNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            try
            {
                var internalContext = new HumanoidNPCGameComponentContext();

                _allowPublicPosition = settings.AllowPublicPosition;
                
                internalContext.IdForFacts = settings.IdForFacts;
                internalContext.SelfInstanceId = settings.InstanceId;

                internalContext.CancellationToken = GetCancellationToken();

                internalContext.AsyncEventsThreadPool = AsyncEventsThreadPool;

                var tmpDir = Path.Combine(worldContext.TmpDir, settings.Id);

                Directory.CreateDirectory(worldContext.TmpDir);

                if(settings.VisionProvider != null)
                {
                    _visionComponent = new VisionComponent(Logger, settings.VisionProvider, internalContext, worldContext);
                    internalContext.VisionComponent = _visionComponent;
                }

                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, worldContext);
                internalContext.HostSupportComponent = _hostSupport;

                _conditionalEntityHostSupportComponent = new ConditionalEntityHostSupportComponent(Logger, settings, _visionComponent, _hostSupport, worldContext);

                _soundPublisher = new SoundPublisherComponent(Logger, settings.InstanceId, settings.IdForFacts, _hostSupport, worldContext);

                _soundReceiverComponent = new SoundReceiverComponent(Logger, settings.InstanceId, internalContext, worldContext);

                _backpackStorage = new ConsolidatedPublicFactsStorage(Logger, KindOfStorage.BackpackStorage);

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.MonitorNode = MonitorNode;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                
                coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
                coreEngineSettings.ParentStorage = worldContext.StandaloneStorage;
                coreEngineSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;
                coreEngineSettings.TmpDir = tmpDir;
                coreEngineSettings.HostListener = this;
                coreEngineSettings.DateTimeProvider = worldContext.DateTimeProvider;
                coreEngineSettings.HostSupport = _hostSupport;
                coreEngineSettings.ConditionalEntityHostSupport = _conditionalEntityHostSupportComponent;
                coreEngineSettings.SoundPublisherProvider = _soundPublisher;
                coreEngineSettings.NLPConverterFactory = worldContext.NLPConverterFactory;

                coreEngineSettings.Categories = settings.Categories;
                coreEngineSettings.EnableCategories = settings.EnableCategories;

                coreEngineSettings.CancellationToken = worldContext.GetCancellationToken();
                coreEngineSettings.ThreadingSettings = settings?.ThreadingSettings ?? worldContext.ThreadingSettings;

                _coreEngine = new Engine(coreEngineSettings);
                internalContext.CoreEngine = _coreEngine;

                _directEngine = _coreEngine;
            }
            catch (Exception e)
            {
                Error("9A9C31DD-54C7-4E82-A6D3-6A84C2ABE746", e);

                throw e;
            }   
        }

        private readonly bool _allowPublicPosition;
        private readonly Engine _coreEngine;
        private readonly IDirectEngine _directEngine;
        private readonly VisionComponent _visionComponent;
        private readonly HostSupportComponent _hostSupport;
        private readonly SoundPublisherComponent _soundPublisher;
        private readonly SoundReceiverComponent _soundReceiverComponent;
        private readonly ConditionalEntityHostSupportComponent _conditionalEntityHostSupportComponent;
        private readonly ConsolidatedPublicFactsStorage _backpackStorage;

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => _coreEngine.PublicFactsStorage;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            _visionComponent?.LoadFromSourceCode();
            _soundReceiverComponent.LoadFromSourceCode();
            _coreEngine.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);

        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
            try
            {
                _coreEngine.BeginStarting();
                _visionComponent?.BeginStarting();
            }
            catch (Exception e)
            {
                Error("20DAE31E-C61E-4121-9735-FCCEAF367ABD", e);

                throw e;
            }           
        }

        /// <inheritdoc/>
        public override bool IsWaited
        {
            get
            {
                if(_visionComponent == null)
                {
                    return _coreEngine.IsWaited;
                }

                return _coreEngine.IsWaited && _visionComponent.IsWaited;
            }
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.OldInsertPublicFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.OldInsertPublicFact(logger, factName, text);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _directEngine.DirectInsertPublicFact(logger, factName, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _coreEngine.OldInsertPublicFact(logger, fact);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _directEngine.DirectInsertPublicFact(logger, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            _coreEngine.OldRemovePublicFact(logger, id);
        }

        public void DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemovePublicFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.OldInsertFact(logger, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.OldInsertFact(logger, factName, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFact(IMonitorLogger logger, string id)
        {
            _coreEngine.OldRemoveFact(logger, id);
        }

        public void DirectRemoveFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemoveFact(logger, id);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, string text)
        {
            _soundPublisher.OldPushSoundFact(power, text);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.OldPushSoundFact(power, fact);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            _coreEngine.OldAddCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _coreEngine.OldAddCategories(logger, categories);
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _directEngine.DirectAddCategories(logger, categories);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            _coreEngine.OldRemoveCategory(logger, category);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _coreEngine.OldRemoveCategories(logger, categories);
        }

        public void DirectRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _directEngine.DirectRemoveCategories(logger, categories);
        }

        public bool EnableCategories { get => _coreEngine.EnableCategories; set => _coreEngine.EnableCategories = value; }

        /// <inheritdoc/>
        public override bool CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            return false;
        }

        /// <inheritdoc/>
        public override Vector3? GetPosition(IMonitorLogger logger)
        {
            if (_allowPublicPosition)
            {
                return _hostSupport.GetCurrentAbsolutePosition(logger);
            }

            return null;
        }

        public IStorage BackpackStorage => _backpackStorage;

        [Obsolete("Serialization Refactoring", true)]
        public void OldAddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.AddConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.RemoveConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _coreEngine.EngineContext;

        [Obsolete("Serialization Refactoring", true)]
        public void OldDie()
        {
            _coreEngine.OldDie();
            _visionComponent?.OldDie();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {           
            _coreEngine.Dispose();
            _visionComponent?.Dispose();
            _soundPublisher.Dispose();
            _soundReceiverComponent.Dispose();
            _hostSupport.Dispose();

            base.OnDisposed();
        }
    }
}
