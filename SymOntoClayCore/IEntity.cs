using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IEntity
    {
        int InstanceId { get; }
        string Id { get; }
        Vector3 Position { get; }
        bool IsEmpty { get; }

        void Specify(params EntityConstraints[] constraints);
    }
}
