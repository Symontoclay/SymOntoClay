using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    /// <summary>
    /// Represents an entity (concrete or conditional).
    /// This interface can be used in host methods.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Returns Instance ID on Unity game object.
        /// It allows to bing SymOntoClay's entity to entity on game level.
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// Gets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets unique Id which is prepared to using in building fact string.
        /// </summary>
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
