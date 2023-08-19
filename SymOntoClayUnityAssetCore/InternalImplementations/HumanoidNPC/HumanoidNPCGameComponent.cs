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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
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
                coreEngineSettings.KindOfLogicalSearchExplain = worldContext.KindOfLogicalSearchExplain;
                coreEngineSettings.LogicalSearchExplainDumpDir = worldContext.LogicalSearchExplainDumpDir;
                coreEngineSettings.EnableAddingRemovingFactLoggingInStorages = worldContext.EnableAddingRemovingFactLoggingInStorages;

                coreEngineSettings.Categories = settings.Categories;
                coreEngineSettings.EnableCategories = settings.EnableCategories;

                _coreEngine = new Engine(coreEngineSettings);
                internalContext.CoreEngine = _coreEngine;
            }
            catch (Exception e)
            {
                Error("9A9C31DD-54C7-4E82-A6D3-6A84C2ABE746", e);

                throw e;
            }   
        }

        private readonly bool _allowPublicPosition;
        private readonly Engine _coreEngine;
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

        public string InsertPublicFact(string text)
        {
            return _coreEngine.InsertPublicFact(text);
        }

        public string InsertPublicFact(RuleInstance fact)
        {
            return _coreEngine.InsertPublicFact(fact);
        }

        public void RemovePublicFact(string id)
        {
            _coreEngine.RemovePublicFact(id);
        }

        public string InsertFact(string text)
        {
            return _coreEngine.InsertFact(text);
        }

        public void RemoveFact(string id)
        {
            _coreEngine.RemoveFact(id);
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
            _coreEngine.AddCategory(category);
        }

        public void AddCategories(List<string> categories)
        {
            _coreEngine.AddCategories(categories);
        }

        public void RemoveCategory(string category)
        {
            _coreEngine.RemoveCategory(category);
        }

        public void RemoveCategories(List<string> categories)
        {
            _coreEngine.RemoveCategories(categories);
        }

        public bool EnableCategories { get => _coreEngine.EnableCategories; set => _coreEngine.EnableCategories = value; }

        /// <inheritdoc/>
        public override bool CanBeTakenBy(IEntity subject)
        {
            return false;
        }

        /// <inheritdoc/>
        public override Vector3? GetPosition()
        {
            if (_allowPublicPosition)
            {
                return _hostSupport.GetCurrentAbsolutePosition();
            }

            return null;
        }

        public IStorage BackpackStorage => _backpackStorage;

        public void AddToBackpack(IGameObject obj)
        {
            _backpackStorage.AddConsolidatedStorage(obj.PublicFactsStorage);
        }

        public void RemoveFromBackpack(IGameObject obj)
        {
            _backpackStorage.RemoveConsolidatedStorage(obj.PublicFactsStorage);
        }

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _coreEngine.EngineContext;

        public void Die()
        {
            _coreEngine.Die();
            _visionComponent?.Die();
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
