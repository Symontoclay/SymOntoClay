/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData.ScriptingData
{
    public enum OperationCode
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        Nop,

        /// <summary>
        /// Pushes a constant value into a current stack.
        /// </summary>
        PushVal,

        /// <summary>
        /// Pushes a value from a variable into a current stack.
        /// </summary>
        PushValFromVar,
        PushValToVar,

        /// <summary>
        /// Calls an unary operator
        /// </summary>
        CallUnOp,

        /// <summary>
        /// Calls a binary operator
        /// </summary>
        CallBinOp,
        Call,
        Call_N,
        Call_P,
        AsyncCall,
        AsyncCall_N,
        AsyncCall_P,

        ClearStack,
        Return,
        ReturnVal,
        UseInheritance,
        UseNotInheritance,
        AllocateNamedWaypoint,
        AllocateAnonymousWaypoint
    }
}
