using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TestSandbox.Helpers
{
    public class TstPlatformSupport: IPlatformSupport
    {
        public Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates)
        {
            return new Vector3(666, 999, 0);
        }
    }
}
