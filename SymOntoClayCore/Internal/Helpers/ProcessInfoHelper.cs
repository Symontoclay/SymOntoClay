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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ProcessInfoHelper
    {
        public static void Wait(IMonitorLogger logger, params IProcessInfo[] processes)
        {
            Wait(logger, null, null, TimeoutCancellationMode.WeakCancel, null, processes);
        }

        public static void Wait(IMonitorLogger logger, List<IExecutionCoordinator> executionCoordinators, long? cancelAfter, TimeoutCancellationMode timeoutCancellationMode, IDateTimeProvider dateTimeProvider, params IProcessInfo[] processes)
        {
            if(processes.IsNullOrEmpty())
            {
                return;
            }

            var initialTicks = 0f;

            if(cancelAfter.HasValue)
            {
                initialTicks = dateTimeProvider.CurrentTiks;
            }

            while(true)
            {
                if(processes.All(p => p.IsFinished))
                {
                    return;
                }
                
                if(executionCoordinators != null)
                {
                    if (executionCoordinators.Any(p => p.ExecutionStatus != ActionExecutionStatus.Executing))
                    {
                        if (executionCoordinators.Any(p => p.ExecutionStatus == ActionExecutionStatus.Canceled))
                        {
                            var cancelledExecutionCoordinatorsChangers = executionCoordinators.Where(p => p.ExecutionStatus == ActionExecutionStatus.Canceled).Select(p => new Changer(KindOfChanger.ExecutionCoordinator, p.Id)).ToList();

                            foreach (var proc in processes)
                            {
                                proc.Cancel(logger, "15E111BA-A585-46AB-A73E-9A554CD128C4", ReasonOfChangeStatus.ByExecutionCoordinator, cancelledExecutionCoordinatorsChangers);
                            }
                        }
                        else
                        {
                            var executionCoordinatorsChangers = executionCoordinators.Select(p => new Changer(KindOfChanger.ExecutionCoordinator, p.Id)).ToList();

                            foreach (var proc in processes)
                            {
                                proc.WeakCancel(logger, "589446A0-232E-4D86-A7F1-4E0A42BC13B8", ReasonOfChangeStatus.ByExecutionCoordinator, executionCoordinatorsChangers);
                            }
                        }

                        return;
                    }
                }

                if(cancelAfter.HasValue)
                {
                    var currentTick = dateTimeProvider.CurrentTiks;
                    
                    var delta = currentTick - initialTicks;

                    if (delta >= cancelAfter.Value)
                    {
                        foreach (var proc in processes)
                        {
                            switch(timeoutCancellationMode)
                            {
                                case TimeoutCancellationMode.WeakCancel:
                                    proc.WeakCancel(logger, "6F69A1A8-5C55-4B33-B711-BDD4C4F39F83", ReasonOfChangeStatus.ByTimeout);
                                    break;

                                case TimeoutCancellationMode.Cancel:
                                    proc.Cancel(logger, "208DD252-4216-40B9-BAAE-5049EBE3ED61", ReasonOfChangeStatus.ByTimeout);
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

        public static int Compare(IMonitorLogger logger, IProcessInfo x, IProcessInfo y)
        {
            if(x.ParentProcessInfo == null && y.ParentProcessInfo == null)
            {
                return ComparePriority(logger, x.Priority, y.Priority);
            }

            var xHierarchyList = ConvertToHierarchyList(logger, x);
            var yHierarchyList = ConvertToHierarchyList(logger, y);

            var xEnumerator = xHierarchyList.GetEnumerator();
            var yEnumerator = yHierarchyList.GetEnumerator();

            float lastXPriority = 0f;
            float lastYPriority = 0f;

            while(true)
            {
                var isFinished = true;

                if (xEnumerator.MoveNext())
                {
                    lastXPriority = xEnumerator.Current.Priority;
                    isFinished = false;
                }

                if (yEnumerator.MoveNext())
                {
                    lastYPriority = yEnumerator.Current.Priority;
                    isFinished = false;
                }

                if (isFinished)
                {
                    return 0;
                }

                if (lastXPriority > lastYPriority)
                {
                    return 1;
                }

                if (lastXPriority < lastYPriority)
                {
                    return -1;
                }
            }
        }

        private static int ComparePriority(IMonitorLogger logger, float x, float y)
        {
            if(x == y)
            {
                return 0;
            }

            if(x > y)
            {
                return 1;
            }

            return -1;
        }

        public static List<IProcessInfo> ConvertToHierarchyList(IMonitorLogger logger, IProcessInfo processInfo)
        {
            var processesInfoList = new List<IProcessInfo>();

            var procI = processInfo;

            do
            {
#if DEBUG
                //DebugLogger.Instance.Info($"processInfo.Id = {processInfo.Id};processInfo.EndPointName = {processInfo.EndPointName}; processInfo.Priority = {processInfo.Priority}; processInfo.Priority = {processInfo.GlobalPriority}");
#endif

                processesInfoList.Add(procI);

                procI = procI.ParentProcessInfo;
            }
            while (procI != null);

            processesInfoList.Reverse();

            return processesInfoList;
        }
    }
}
