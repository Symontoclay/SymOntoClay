using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core
{
    /// <summary>
    /// Represents an entity (concrete or conditional).
    /// This interface can be used in host-methods.
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

        /// <summary>
        /// Gets position of the entity.
        /// </summary>
        Vector3? Position { get; }

        /// <summary>
        /// Returns <b>true</b> if the entity is not resolved, otherwise returns <b>false</b>.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Sets constraints that will be used during resolving.
        /// It allows to resolve entity in way which is fit for the host-method.
        /// These constraints will be used every resolving.
        /// </summary>
        /// <param name="constraints">Array of constraints.</param>
        void Specify(params EntityConstraints[] constraints);

        /// <summary>
        /// Sets constraints that will be used during resolving.
        /// It allows to resolve entity in way which is fit for the host-method.
        /// These constraints will be used once.
        /// Next resolving will not use these constraints.
        /// </summary>
        /// <param name="constraints">Array of constraints.</param>
        void SpecifyOnce(params EntityConstraints[] constraints);

        /// <summary>
        /// Sets backpack storage that will be used during resolving.
        /// It allows to resolve entity in way which is fit for the host-method.
        /// This backpack storage will be used once.
        /// Next resolving will not use this backpack storage.
        /// </summary>
        /// <param name="storage">Backpack storage</param>
        void SpecifyOnce(IStorage storage);

        /// <summary>
        /// Resolves (finds) entity with constraints or backpack storage.
        /// If entity has been previously resolved, the more fittable entity will has been found.
        /// </summary>
        void Resolve();

        /// <summary>
        /// Resolves (finds) entity with constraints or backpack storage.
        /// If entity has been previously resolved, the method does nothing.
        /// </summary>
        void ResolveIfNeeds();
    }
}
