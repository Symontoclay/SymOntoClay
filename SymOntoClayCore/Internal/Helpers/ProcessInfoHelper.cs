using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ProcessInfoHelper
    {
        public static void Wait(params IProcessInfo[] processes)
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

                Thread.Sleep(100);
            }
        }
    }
}
