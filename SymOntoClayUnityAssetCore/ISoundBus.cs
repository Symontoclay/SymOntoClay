using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface ISoundBus
    {
        void AddReceiver(ISoundReceiver receiver);
        void RemoveReceiver(ISoundReceiver receiver);
        void PushSound(int instanceId, float power, Vector3 position, string query);
    }
}
