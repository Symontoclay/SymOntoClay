using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IConditionalEntityHostSupport
    {
        int GetInstanceId(StrongIdentifierValue id);
        bool IsVisible(int instanceId);
        bool CanBeTaken(int instanceId);
        Vector3? GetPosition(int instanceId);
        (float?, Vector3?) DistanceToAndPosition(int instanceId);
        float? DistanceTo(int instanceId);
    }
}
