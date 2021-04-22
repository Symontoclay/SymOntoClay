using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClayDefaultCLIEnvironment
{
    public static class DefaultInvokerInMainThreadFactory
    {
        public static InvokerInMainThread Create()
        {
            var invokingInMainThread = new InvokerInMainThread();

            Task.Run(() => {
                while (true)
                {
                    invokingInMainThread.Update();

                    Thread.Sleep(1000);
                }
            });

            return invokingInMainThread;
        }
    }
}
