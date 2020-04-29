using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents runtime image of executed code.
    /// </summary>
    public interface IRunTimeImageInfo
    {
        /// <summary>
        /// Gets image id.
        /// It should be Universally Unique Identifier (UUID).
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets creation time.
        /// </summary>
        DateTime CreationTime { get; }

        /// <summary>
        /// Gets user defined description of the image.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets parent image id or <c>null</c> if the image was created directly from source code.
        /// </summary>
        string ParentId { get; }
    }
}
