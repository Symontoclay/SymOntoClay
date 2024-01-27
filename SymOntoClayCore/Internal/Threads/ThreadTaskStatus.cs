using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    /// <summary>
    /// Represents the current stage in the lifecycle of a <see cref="ThreadTask"/>.
    /// </summary>
    public enum ThreadTaskStatus
    {
        /// <summary>
        /// The task has been initialized but has not yet been scheduled.
        /// </summary>
        Created = 0,
        /// <summary>
        /// The task is waiting to be activated and scheduled.
        /// </summary>
        WaitingForActivation = 1,
        /// <summary>
        /// The task has been scheduled for execution but has not yet begun executing.
        /// </summary>
        WaitingToRun = 2,
        /// <summary>
        /// The task is running but has not yet completed.
        /// </summary>
        Running = 3,
        /// <summary>
        /// The task has finished executing and is implicitly waiting for attached child tasks to complete.
        /// </summary>
        WaitingForChildrenToComplete = 4,
        /// <summary>
        /// The task completed execution successfully.
        /// </summary>
        RanToCompletion = 5,
        /// <summary>
        /// The task acknowledged cancellation by throwing an <see cref="OperationCanceledException"/> with its own <see cref="CancellationToken"/> while the token was in signaled state, or the task's <see cref="CancellationToken"/> was already signaled before the task started executing.
        /// </summary>
        Canceled = 6,
        /// <summary>
        /// The task completed due to an unhandled exception.
        /// </summary>
        Faulted = 7
    }
}
