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

using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ProcessInfoHelper
    {
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
