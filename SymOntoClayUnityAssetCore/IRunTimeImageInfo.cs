/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
