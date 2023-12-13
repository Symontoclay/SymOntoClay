using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.CoreHostListener
{
    public class TstPlatformHostListenerWithDefaultValues
    {
        [DebuggerHidden]
        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction, float speed = 2)
        {
#if DEBUG
            logger.Info("768F6328-1015-4862-8ADA-45560D425970", $"RotateImpl Begin direction = {direction}; speed = {speed}");
#endif



#if DEBUG
            logger.Info("8B9AB25B-EACA-4992-A807-647E54942C94", $"RotateImpl End");
#endif
        }
    }
}
