/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public abstract class BaseStoredGameComponent: BaseGameComponent
    {
        protected BaseStoredGameComponent(BaseStoredGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            try
            {
                var standaloneStorageSettings = new StandaloneStorageSettings();
                standaloneStorageSettings.Id = settings.Id;
                standaloneStorageSettings.IsWorld = false;
                standaloneStorageSettings.AppFile = settings.HostFile;
                standaloneStorageSettings.Logger = Logger;
                standaloneStorageSettings.Dictionary = worldContext.SharedDictionary;
                standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage;
                standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage;

#if DEBUG
                //Log($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
                HostStorage = new StandaloneStorage(standaloneStorageSettings);
            }
            catch (Exception e)
            {
                Log(e.ToString());

                throw e;
            }
        }

        protected StandaloneStorage HostStorage { get; private set; }

        /// <inheritdoc/>
        public override void LoadFromSourceCode()
        {
            base.LoadFromSourceCode();

            //try
            //{
                HostStorage.LoadFromSourceCode();
            //}
            //catch (Exception e)
            //{
            //    Log(e.ToString());

            //    throw e;
            //}
        }

        public string InsertFact(string text)
        {
            if(!text.StartsWith("{:"))
            {
                text = $"{{: {text} :}}";
            }

            return HostStorage.InsertFact(text);
        }

        public void RemoveFact(string id)
        {
            HostStorage.RemoveFact(id);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            HostStorage.Dispose();

            base.OnDisposed();
        }
    }
}
