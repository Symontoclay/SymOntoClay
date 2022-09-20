using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    public interface INavTarget
    {
        KindOfNavTarget Kind { get; }
        float Distance { get; }
        float HorizontalAngle { get; }
        Vector3 AbcoluteCoordinates { get; }
        IEntity Entity { get; }
    }
}
