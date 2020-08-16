using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core.Internal.PlatformSupport
{
    public interface IPlatformSupport
    {
        Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates);
    }
}
