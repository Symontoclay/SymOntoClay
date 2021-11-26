using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    /// <summary>
    /// Constraints inform the entity resolver about the capabilities an conditional entity must have on game level.
    /// Without any constraints, the conditional entity could be anything on game level.
    /// Сonstraints allow to resolve conditional entity to entity on game level that is the fittest to the host method.
    /// </summary>
    public enum EntityConstraints
    {
        /// <summary>
        /// Entity on game level can be taken by NPC's hands.
        /// For example, entities which are held by another NPC will be ignored.
        /// </summary>
        CanBeTaken,

        /// <summary>
        /// Entity on game level should be visible for the NPC.
        /// Invisible entities will be ignored.
        /// </summary>
        OnlyVisible,

        /// <summary>
        /// Entity on game level should not be visible for the NPC.
        /// Visible entities will be ignored.
        /// </summary>
        OnlyInvisible,

        /// <summary>
        /// The nearest entity on game level will be preferred.
        /// </summary>
        Nearest
    }
}
