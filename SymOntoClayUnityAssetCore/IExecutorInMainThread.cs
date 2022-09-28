using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IExecutorInMainThread
    {
        void RunInMainThread(Action function);
        TResult RunInMainThread<TResult>(Func<TResult> function);
    }
}
