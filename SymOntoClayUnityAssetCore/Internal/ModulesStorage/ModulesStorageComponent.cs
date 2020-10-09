/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.ModulesStorage
{
    public class ModulesStorageComponent : BaseWorldCoreComponent
    {
        public ModulesStorageComponent(IWorldCoreContext coreContext)
            : base(coreContext)
        {
            var modulesStorageSettings = new ModulesStorageSettings();
            modulesStorageSettings.Logger = Logger;
            modulesStorageSettings.Dictionary = coreContext.SharedDictionary;

            Log($"modulesStorageSettings = {modulesStorageSettings}");

            _modulesStorage = new SymOntoClay.Core.ModulesStorage(modulesStorageSettings);
        }

        private readonly SymOntoClay.Core.ModulesStorage _modulesStorage;

        public SymOntoClay.Core.IModulesStorage ModulesStorage => _modulesStorage;

        public void LoadFromSourceCode()
        {
            _modulesStorage.LoadFromSourceCode();
        }
    }
}
