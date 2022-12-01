/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    /// <summary>
    /// Constraints inform the entity resolver about the capabilities an conditional entity must have on game level.
    /// Without any constraints, the conditional entity could be anything on game level.
    /// Constraints allow to resolve conditional entity to entity on game level that is the fittest to the host method.
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
        Nearest,

        /// <summary>
        /// A random entity on game level will be preferred.
        /// </summary>
        Random
    }
}
