using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Player
{
    public class PlayerGameComponent : BaseStoredGameComponent
    {
        public PlayerGameComponent(PlayerSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            return HostStorage.InsertPublicFact(text);
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            HostStorage.RemovePublicFact(id);
        }

        /// <inheritdoc/>
        public override bool IsWaited => true;
    }
}
