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
        string IdForFacts { get; }
        Vector3? Position { get; }
        bool IsEmpty { get; }

        void Specify(params EntityConstraints[] constraints);
        void SpecifyOnce(params EntityConstraints[] constraints);
        void SpecifyOnce(IStorage storage);

        void Resolve();
        void ResolveIfNeeds();
    }
}
