/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.Core.Internal.CodeExecution.Helpers
{
    public static class CodeFrameStateHelper
    {
        /// <summary>
        /// Returns true if command execution can be begun, otherwise returns false.
        /// </summary>
        /// <param name="currentCodeFrameState"></param>
        /// <returns>True if command execution can be begun, otherwise returns false.</returns>
        public static bool CanBeginCommandExecution(CodeFrameState currentCodeFrameState)
        {
            return currentCodeFrameState == CodeFrameState.Init ||
                currentCodeFrameState == CodeFrameState.EndCommandExecution;
        }

        /// <summary>
        /// Returns true if a taking caller method should be called, otherwise returns false.
        /// </summary>
        /// <param name="currentCodeFrameState">Current code frame state.</param>
        /// <returns>True if a taking caller method should be called, otherwise returns false.</returns>
        public static bool ShouldCallTakeCaller(CodeFrameState currentCodeFrameState)
        {
            return currentCodeFrameState == CodeFrameState.BeginningCommandExecution ||
                currentCodeFrameState == CodeFrameState.TakingCaller ||
                currentCodeFrameState == CodeFrameState.ResolvingCallerInCodeFrame;
        }

        /// <summary>
        /// Returns true if a taking parameters method should be called, otherwise returns false.
        /// </summary>
        /// <param name="currentCodeFrameState">Current code frame state.</param>
        /// <param name="needCheckingBeginningCommandExecution">Needs to check case with CodeFrameState.BeginningCommandExecution or not.</param>
        /// <returns>True if a taking parameters method should be called, otherwise returns false.</returns>
        public static bool ShouldCallTakeParameters(CodeFrameState currentCodeFrameState, bool needCheckingBeginningCommandExecution = true)
        {
            return (needCheckingBeginningCommandExecution && currentCodeFrameState == CodeFrameState.BeginningCommandExecution) ||
                currentCodeFrameState == CodeFrameState.TakingParameters ||
                currentCodeFrameState == CodeFrameState.ResolvingParameterInCodeFrame;
        }
    }
}
