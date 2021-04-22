using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClayDefaultCLIEnvironment
{
    public class PlatformSupportCLIStub : IPlatformSupport
    {
        public Vector3 ConvertFromRelativeToAbsolute(RelativeCoordinate relativeCoordinate)
        {
            return new Vector3(666, 999, 0);
        }
    }
}
