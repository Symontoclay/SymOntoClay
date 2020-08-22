using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IHostSupport
    {
        Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates);
    }
}
