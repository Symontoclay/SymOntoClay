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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC
{
    public class HumanoidNPCGameComponent: BaseManualControllingGameComponent
    {
        public HumanoidNPCGameComponent(HumanoidNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext, KindOfWorldItem.HumanoidNPC)
        {
            try
            {
                _settings = settings;

                var internalContext = new HumanoidNPCGameComponentContext();
                _internalContext = internalContext;

                _allowPublicPosition = settings.AllowPublicPosition;
                
                internalContext.IdForFacts = settings.IdForFacts;
                internalContext.SelfInstanceId = settings.InstanceId;

                internalContext.CancellationContext = GetCancellationContext();

                internalContext.AsyncEventsThreadPool = AsyncEventsThreadPool;

                var tmpDir = Path.Combine(worldContext.TmpDir, settings.Id);
                internalContext.TmpDir = tmpDir;

                Directory.CreateDirectory(worldContext.TmpDir);

                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, worldContext);
                internalContext.HostSupportComponent = _hostSupport;

                _soundPublisher = new SoundPublisherComponent(Logger, settings.InstanceId, settings.IdForFacts, _hostSupport, worldContext);
                internalContext.SoundPublisherComponent = _soundPublisher;
            }
            catch (Exception e)
            {
                Error("9A9C31DD-54C7-4E82-A6D3-6A84C2ABE746", e);

                throw e;
            }
        }

        private readonly HumanoidNPCSettings _settings;
        private readonly HumanoidNPCGameComponentContext _internalContext;
        private HumanoidNPCGameComponentSerializedContext _internalSerializedContext;
        private readonly bool _allowPublicPosition;
        
        //private readonly Engine _coreEngine;
        //private readonly VisionComponent _visionComponent;
        private readonly HostSupportComponent _hostSupport;
        private readonly SoundPublisherComponent _soundPublisher;
        //private readonly SoundReceiverComponent _soundReceiverComponent;
        //private readonly ConditionalEntityHostSupportComponent _conditionalEntityHostSupportComponent;
        //private readonly ConsolidatedPublicFactsStorage _backpackStorage;

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => _coreEngine.PublicFactsStorage;

        private void CreateSerializedComponents()
        {
            _internalSerializedContext?.Dispose();
            _internalSerializedContext = new HumanoidNPCGameComponentSerializedContext(this, _settings, _internalContext, _worldContext);
        }

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            CreateSerializedComponents();

            _internalSerializedContext?.LoadFromSourceCode();

            _worldContext.AddPublicFactsStorage(this);
        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
            try
            {
                _internalSerializedContext?.BeginStarting();
            }
            catch (Exception e)
            {
                Error("20DAE31E-C61E-4121-9735-FCCEAF367ABD", e);

                throw e;
            }           
        }

        /// <inheritdoc/>
        public override void EndStarting()
        {
            try
            {
                _internalSerializedContext?.EndStarting();
            }
            catch (Exception e)
            {
                Error("3971D614-2C6A-4D57-9D5D-DCAC2805D9D1", e);

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

        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertPublicFact(logger, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertPublicFact(logger, factName, text);
        }

        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _coreEngine.InsertPublicFact(logger, fact);
        }

        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            _coreEngine.RemovePublicFact(logger, id);
        }

        public string InsertFact(IMonitorLogger logger, string text)
        {
            return _coreEngine.InsertFact(logger, text);
        }

        public string InsertFact(IMonitorLogger logger, StrongIdentifierValue factName, string text)
        {
            return _coreEngine.InsertFact(logger, factName, text);
        }

        public void RemoveFact(IMonitorLogger logger, string id)
        {
            _coreEngine.RemoveFact(logger, id);
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
            _coreEngine.AddCategory(logger, category);
        }

        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            _coreEngine.AddCategories(logger, categories);
        }

        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            _coreEngine.RemoveCategory(logger, category);
        }

        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _coreEngine.RemoveCategories(logger, categories);
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

        public void AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.AddConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        public void RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _backpackStorage.RemoveConsolidatedStorage(logger, obj.PublicFactsStorage);
        }

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        public IEngineContext EngineContext => _coreEngine.EngineContext;

        public void Die()
        {
            _internalSerializedContext?.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _internalSerializedContext?.Dispose();

            _soundPublisher.Dispose();
            
            _hostSupport.Dispose();

            base.OnDisposed();
        }
    }
}
