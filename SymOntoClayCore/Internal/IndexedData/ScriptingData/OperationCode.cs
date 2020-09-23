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
