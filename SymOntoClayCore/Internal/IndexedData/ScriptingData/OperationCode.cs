/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
        /// Calls an unary operator.
        /// </summary>
        CallUnOp,

        /// <summary>
        /// Calls a binary operator.
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
        SetInheritance,
        SetNotInheritance,
        Error,
        SetSEHGroup,
        RemoveSEHGroup,
        /// <summary>
        /// Jump to target position.
        /// </summary>
        JumpTo,
        /// <summary>
        /// Jump to target position if top stack value equals <b>true</b> (1).
        /// </summary>
        JumpToIfTrue,
        /// <summary>
        /// Jump to target position if top stack value equals <b>false</b> (0).
        /// </summary>
        JumpToIfFalse,
        Await,
        CompleteAction,
        CompleteActionVal,
        BreakAction,
        BreakActionVal,
        VarDecl
    }
}
