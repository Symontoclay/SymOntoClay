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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
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

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertPublicFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertPublicFact(logger, factName, text);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _directEngine.DirectInsertPublicFact(logger, factName, text);
        }

        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _coreEngine.InsertPublicFact(logger, fact);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _directEngine.DirectInsertPublicFact(logger, fact);
        }

        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _coreEngine.RemovePublicFact(logger, id);
        }

        public void DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemovePublicFact(logger, id);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertFact(logger, text);
        }

        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertFact(logger, factName, text);
        }

        public string DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _directEngine.DirectInsertFact(logger, factName, text);
        }

        public ISyncMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return _coreEngine.RemoveFact(logger, id);
        }

        public void DirectRemoveFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemoveFact(logger, id);
        }

        public ISyncMethodResponse PushSoundFact(float power, string text)
        {
            return LoggedSyncFunctorWithoutResult<HumanoidNPCGameComponent, float, string>.Run(Logger, "B568048D-680D-4DE3-B3CC-4B16A72AAFBF", this, power, text,
                (IMonitorLogger loggerValue, HumanoidNPCGameComponent instanceValue, float powerValue, string textValue) => {
                    instanceValue.NPushSoundFact(powerValue, textValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NPushSoundFact(float power, string text)
        {
            _soundPublisher.PushSoundFact(power, text);
        }

        public ISyncMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            return LoggedSyncFunctorWithoutResult<HumanoidNPCGameComponent, float, RuleInstance>.Run(Logger, "24F76F7D-76E6-4417-B1FB-9C8CE3986E32", this, power, fact,
                (IMonitorLogger loggerValue, HumanoidNPCGameComponent instanceValue, float powerValue, RuleInstance factValue) => {
                    instanceValue.NPushSoundFact(powerValue, factValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NPushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.PushSoundFact(power, fact);
        }

        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _coreEngine.AddCategory(logger, category);
        }

        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _coreEngine.AddCategories(logger, categories);
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _directEngine.DirectAddCategories(logger, categories);
        }

        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _coreEngine.RemoveCategory(logger, category);
        }

        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            return _coreEngine.RemoveCategories(logger, categories);
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

        public ISyncMethodResponse AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            return LoggedSyncFunctorWithoutResult<HumanoidNPCGameComponent, IGameObject>.Run(logger, "17F2084B-201C-486A-A94F-ADE42188AB9E", this, obj,
                (IMonitorLogger loggerValue, HumanoidNPCGameComponent instanceValue, IGameObject objValue) => {
                    instanceValue.NAddToBackpack(loggerValue, objValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NAddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.AddConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        public ISyncMethodResponse RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            return LoggedSyncFunctorWithoutResult<HumanoidNPCGameComponent, IGameObject>.Run(logger, "19E62F24-5F37-4853-9423-9DB87FB8D061", this, obj,
                (IMonitorLogger loggerValue, HumanoidNPCGameComponent instanceValue, IGameObject objValue) => {
                    instanceValue.NRemoveFromBackpack(loggerValue, objValue);
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NRemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.RemoveConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _coreEngine.EngineContext;

        public ISyncMethodResponse Die()
        {
            return LoggedSyncFunctorWithoutResult<HumanoidNPCGameComponent>.Run(Logger, "6C3E7934-1F44-4AD8-A0CE-EDB068DDE8F9", this,
                (IMonitorLogger loggerValue, HumanoidNPCGameComponent instanceValue) => {
                    instanceValue.NDie();
                },
                _activeObjectContext, _serializationAnchor).ToMethodResponse();
        }

        public void NDie()
        {
            _directEngine.DirectDie();
            _visionComponent?.DirectDie();
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
