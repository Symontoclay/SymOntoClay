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
        }

        private readonly EndpointsRegistry _hostEndpointsRegistry;

        public IEndpointsRegistry EndpointsRegistry => _hostEndpointsRegistry;
    }
}
