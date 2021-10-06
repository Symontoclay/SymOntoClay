using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.SoundBusHandler
{
    public interface ISoundBus
    {
        void AddReceiver(ISoundReceiver receiver);
        void RemoveReceiver(ISoundReceiver receiver);
        void PushSound(double power, Vector3 position, string query);
    }
}
