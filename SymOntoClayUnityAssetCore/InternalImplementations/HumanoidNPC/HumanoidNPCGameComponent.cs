/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.IO;
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
#if DEBUG
                //Log($"settings = {settings}");
                //Log($"worldContext.TmpDir = {worldContext.TmpDir}");
                //Log($"worldContext.LogicQueryParseAndCache == null = {worldContext.LogicQueryParseAndCache == null}");
#endif

                var internalContext = new HumanoidNPCGameComponentContext();

                _idForFacts = settings.IdForFacts;

                internalContext.IdForFacts = _idForFacts;
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

                _soundPublisher = new SoundPublisherComponent(Logger, settings.InstanceId, _hostSupport, worldContext);

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.Logger = Logger;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                
                coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
                coreEngineSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;
                coreEngineSettings.TmpDir = tmpDir;
                coreEngineSettings.HostListener = this;
                coreEngineSettings.DateTimeProvider = worldContext.DateTimeProvider;
                coreEngineSettings.HostSupport = _hostSupport;

#if DEBUG
                //Log($"coreEngineSettings = {coreEngineSettings}");
#endif
                _coreEngine = new Engine(coreEngineSettings);
                internalContext.CoreEngine = _coreEngine;
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }   
        }

        private readonly string _idForFacts;
        private readonly Engine _coreEngine;
        private readonly VisionComponent _visionComponent;
        private readonly HostSupportComponent _hostSupport;
        private readonly SoundPublisherComponent _soundPublisher;

        /// <inheritdoc/>
        public override IStorage PublicFactsStorage => _coreEngine.PublicFactsStorage;

        /// <inheritdoc/>
        public override string IdForFacts => _idForFacts;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
            _visionComponent?.LoadFromSourceCode();
                _coreEngine.LoadFromSourceCode();
            //}
            //catch(Exception e)
            //{
                //Log(e.ToString());

            //    throw e;
            //}
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
                Log(e.ToString());

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
            _hostSupport.Dispose();

            base.OnDisposed();
        }
    }
}
