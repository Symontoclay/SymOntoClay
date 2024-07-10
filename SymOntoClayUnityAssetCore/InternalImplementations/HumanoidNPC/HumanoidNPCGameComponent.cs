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
using SymOntoClay.Core.Internal.Serialization.Functors;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;

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
                _threadPool = AsyncEventsThreadPool;
                _activeObjectContext = ActiveObjectContext;

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

                coreEngineSettings.CancellationToken = GetCancellationToken();
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
        private readonly IActiveObjectContext _activeObjectContext;
        private readonly ICustomThreadPool _threadPool;

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

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertPublicFact(logger, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertPublicFact(logger, factName, text);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _directEngine.DirectInsertPublicFact(logger, factName, text);
        }

        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _coreEngine.InsertPublicFact(logger, fact);
        }

        public string DirectInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _directEngine.DirectInsertPublicFact(logger, fact);
        }

        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _coreEngine.RemovePublicFact(logger, id);
        }

        public void DirectRemovePublicFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemovePublicFact(logger, id);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertFact(logger, text);
        }

        public IMethodResponse<string> InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertFact(logger, factName, text);
        }

        public string DirectInsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _directEngine.DirectInsertFact(logger, factName, text);
        }

        public IMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            return _coreEngine.RemoveFact(logger, id);
        }

        public void DirectRemoveFact(IMonitorLogger logger, string id)
        {
            _directEngine.DirectRemoveFact(logger, id);
        }

        public IMethodResponse PushSoundFact(float power, string text)
        {
            _soundPublisher.PushSoundFact(power, text);
        }

        public IMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            _soundPublisher.PushSoundFact(power, fact);
        }

        public IMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            return _coreEngine.AddCategory(logger, category);
        }

        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            return _coreEngine.AddCategories(logger, categories);
        }

        public void DirectAddCategories(IMonitorLogger logger, List<string> categories)
        {
            _directEngine.DirectAddCategories(logger, categories);
        }

        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            return _coreEngine.RemoveCategory(logger, category);
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
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

        public IMethodResponse AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.AddConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        public IMethodResponse RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.RemoveConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _coreEngine.EngineContext;

        public IMethodResponse Die()
        {
            return LoggedFunctorWithoutResult.Run(Logger, (loggerValue) => {
                _directEngine.DirectDie();
                _visionComponent?.DirectDie();
            }, _activeObjectContext, _threadPool).ToMethodResponse();
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
