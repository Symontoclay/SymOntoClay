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
        public static void Wait(params IProcessInfo[] processes)
        {
            Wait(null, processes);
        }

        public static void Wait(List<IExecutionCoordinator> executionCoordinators, params IProcessInfo[] processes)
        {
            if(processes.IsNullOrEmpty())
            {
                return;
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

                Thread.Sleep(100);
            }
        }
    }
}
