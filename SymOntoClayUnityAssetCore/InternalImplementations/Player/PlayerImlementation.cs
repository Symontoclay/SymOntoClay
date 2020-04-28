using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Player
{
    public class PlayerImlementation : IPlayer
    {
        public PlayerImlementation(PlayerSettings settings, WorldContext context)
        {

        }

        public bool IsDisposed => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
