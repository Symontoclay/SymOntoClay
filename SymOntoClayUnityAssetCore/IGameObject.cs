using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents each game object which is not NPC, Player or Place.
    /// </summary>
    public interface IGameObject: IWorldComponent
    {
        IEndpointsRegistry EndpointsRegistry { get; }
    }
}
