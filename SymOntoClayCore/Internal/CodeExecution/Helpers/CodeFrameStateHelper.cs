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
