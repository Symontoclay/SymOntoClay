using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class VeryShortMehod_HostListener : BaseHostListener
    {
        [DebuggerHidden]
        [BipedEndpoint("SomeVeryShortSilentFun", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void SomeVeryShortSilentFunImpl(CancellationToken cancellationToken)
        {
            //_logger.Log("SomeVeryShortSilentFunImpl Begin");

            //Sleep(1000, cancellationToken);

            //_logger.Log("SomeVeryShortSilentFunImpl End");
        }
    }
}
