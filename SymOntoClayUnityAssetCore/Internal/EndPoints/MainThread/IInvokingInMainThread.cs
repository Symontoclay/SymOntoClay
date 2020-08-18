using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread
{
    public interface IInvokingInMainThread
    {
        void SetInvocableObj(IInvocableInMainThread invokableObj);
    }
}
