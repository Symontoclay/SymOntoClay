using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class HostMethods_Tests_HostListener: ILoggedTestHostListener
    {
        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
        }

        private IEntityLogger _logger;

        private static object _lockObj = new object();

        private static int _methodId;

        private int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            _logger.Log($"RotateImpl Begin");
            _logger.Log(direction.ToString());
        }
    }
}
