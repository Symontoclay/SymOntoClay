using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public interface IInvokerInMainThread
    {
        void SetInvocableObj(IInvocableInMainThread invokableObj);
    }
}
