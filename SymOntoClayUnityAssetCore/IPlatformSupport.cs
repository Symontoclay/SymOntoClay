using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IPlatformSupport
    {
        Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates);
    }
}
