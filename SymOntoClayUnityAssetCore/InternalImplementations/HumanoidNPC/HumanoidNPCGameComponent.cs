/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.HostSupport;
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
#endif

                var tmpDir = Path.Combine(worldContext.TmpDir, settings.Id);

                Directory.CreateDirectory(worldContext.TmpDir);

                _hostSupport = new HostSupportComponent(Logger, settings.PlatformSupport, worldContext);

                var coreEngineSettings = new EngineSettings();
                coreEngineSettings.Id = settings.Id;
                coreEngineSettings.AppFile = settings.LogicFile;
                coreEngineSettings.Logger = Logger;
                coreEngineSettings.SyncContext = worldContext.SyncContext;
                coreEngineSettings.Dictionary = worldContext.SharedDictionary;
                coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
                coreEngineSettings.TmpDir = tmpDir;
                coreEngineSettings.HostListener = this;
                coreEngineSettings.DateTimeProvider = worldContext.DateTimeProvider;
                coreEngineSettings.HostSupport = _hostSupport;

#if DEBUG
                //Log($"coreEngineSettings = {coreEngineSettings}");
#endif
                _coreEngine = new Engine(coreEngineSettings);
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }   
        }

        private readonly Engine _coreEngine;
        private readonly HostSupportComponent _hostSupport;

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
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
                return _coreEngine.IsWaited;
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

        /// <inheritdoc/>
        protected override void OnDisposed()
        {           
            _coreEngine.Dispose();

            base.OnDisposed();
        }
    }
}
