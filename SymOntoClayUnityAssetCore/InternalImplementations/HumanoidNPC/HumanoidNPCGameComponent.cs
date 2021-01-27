/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
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

                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, internalContext, worldContext);
                internalContext.HostSupportComponent = _hostSupport;

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.Logger = Logger;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                coreEngineSettings.Dictionary = worldContext.SharedDictionary;
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

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            return _coreEngine.InsertPublicFact(text);
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            _coreEngine.RemovePublicFact(id);
        }

        public void Die()
        {
            _coreEngine.Die();
            _visionComponent.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {           
            _coreEngine.Dispose();
            _visionComponent.Dispose();
            _hostSupport.Dispose();

            base.OnDisposed();
        }
    }
}
