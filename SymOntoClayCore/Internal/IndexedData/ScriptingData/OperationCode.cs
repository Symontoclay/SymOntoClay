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

        /// <summary>
        /// Calls an unary operator
        /// </summary>
        CallUnOp,

        /// <summary>
        /// Calls a binary operator
        /// </summary>
        CallBinOp,

        ClearStack,
        Return,
        ReturnVal,
        UseInheritance,
        UseNotInheritance
    }
}
