﻿using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class VeryLongMehod_HostListener: BaseHostListener
    {
        [BipedEndpoint("SomeVeryLongSilentFun", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void SomeVeryLongSilentFunImpl(CancellationToken cancellationToken)
        {
            //_logger.Log("SomeVeryLongSilentFunImpl Begin");

            Sleep(1000, cancellationToken);
        }
    }
}
