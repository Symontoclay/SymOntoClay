using NLog;
using SymOntoClay.SoundBuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.SoundBusHandler
{
    public class TstSoundBusHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var bus = new SimpleSoundBus();

            var receiver1 = new TstSoundReceiver(12, new Vector3(10, 10, 10));

            bus.AddReceiver(receiver1);

            bus.PushSound(12, 60, new Vector3(1,1,1), "act(M16, shoot)");

            _logger.Info("End");
        }
    }
}
