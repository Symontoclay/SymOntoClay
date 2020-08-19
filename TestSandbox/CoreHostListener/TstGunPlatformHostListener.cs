using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CoreHostListener
{
    public class TstGunPlatformHostListener
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        [BipedEndpoint]
        public void Shoot(CancellationToken cancellationToken)
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }
    }
}
