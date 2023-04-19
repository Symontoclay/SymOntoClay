/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ProcessInfoHelper
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static void Wait(params IProcessInfo[] processes)
        {
            Wait(null, null, TimeoutCancellationMode.WeakCancel, null, processes);
        }

        public static void Wait(List<IExecutionCoordinator> executionCoordinators, long? cancelAfter, TimeoutCancellationMode timeoutCancellationMode, IDateTimeProvider dateTimeProvider, params IProcessInfo[] processes)
        {
            if(processes.IsNullOrEmpty())
            {
                return;
            }

            var initialTicks = 0f;

            if(cancelAfter.HasValue)
            {
                var currentTick = dateTimeProvider.CurrentTiks;
                initialTicks = currentTick * dateTimeProvider.MillisecondsMultiplicator;
            }

            while(true)
            {
                if(processes.All(p => p.IsFinished))
                {
                    return;
                }
                
                if(executionCoordinators != null)
                {
                    if(executionCoordinators.Any(p => p.ExecutionStatus != ActionExecutionStatus.Executing))
                    {
                        foreach(var proc in processes)
                        {
                            proc.Cancel();
                        }

                        return;
                    }
                }

                if(cancelAfter.HasValue)
                {
                    var currentTick = dateTimeProvider.CurrentTiks;
                    var currentMilisecond = currentTick * dateTimeProvider.MillisecondsMultiplicator;

                    var delta = currentMilisecond - initialTicks;

#if DEBUG
                    //_gbcLogger.Info($"cancelAfter.Value = {cancelAfter.Value}");
                    //_gbcLogger.Info($"initialTicks = {initialTicks}");
                    //_gbcLogger.Info($"currentTick = {currentTick}");
                    //_gbcLogger.Info($"currentMilisecond = {currentMilisecond}");
                    //_gbcLogger.Info($"delta = {delta}");
#endif

                    if (delta >= cancelAfter.Value)
                    {
#if DEBUG
                        //_gbcLogger.Info($"delta >= cancelAfter.Value !!!!!");
                        //_gbcLogger.Info($"timeoutCancellationMode = {timeoutCancellationMode}");
#endif

                        foreach (var proc in processes)
                        {
                            switch(timeoutCancellationMode)
                            {
                                case TimeoutCancellationMode.WeakCancel:
                                    proc.WeakCancel();
                                    break;

                                case TimeoutCancellationMode.Cancel:
                                    proc.Cancel();
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(timeoutCancellationMode), timeoutCancellationMode, null);
                            }                            
                        }

                        return;
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
