/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject
{
    public class GameObjectGameComponent: BaseStoredGameComponent
    {
        public GameObjectGameComponent(GameObjectSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            _hostEndpointsRegistry = new EndpointsRegistry(Logger);

            var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(settings.HostListener);

            _hostEndpointsRegistry.AddEndpointsRange(platformEndpointsList);

            var publicFactsStorageSettings = new QuickStorageSettings();
            publicFactsStorageSettings.Logger = Logger;
            publicFactsStorageSettings.Dictionary = worldContext.SharedDictionary;
            publicFactsStorageSettings.DateTimeProvider = worldContext.DateTimeProvider;
            publicFactsStorageSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;

            _publicFactsStorage = new QuickStorage(publicFactsStorageSettings);
        }

        private readonly QuickStorage _publicFactsStorage;

        private readonly EndpointsRegistry _hostEndpointsRegistry;

        public IEndpointsRegistry EndpointsRegistry => _hostEndpointsRegistry;

        /// <inheritdoc/>
        public string InsertFact(string text)
        {
            return _publicFactsStorage.InsertFact(text);
        }

        /// <inheritdoc/>
        public void RemoveFact(string id)
        {
            _publicFactsStorage.RemoveFact(id);
        }
    }
}
