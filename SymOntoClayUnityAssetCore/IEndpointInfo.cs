using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IEndpointInfo: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Name { get; }
        bool NeedMainThread { get; }
        IList<int> Devices { get; }
        IList<IEndpointArgumentInfo> Arguments { get; }
        MethodInfo MethodInfo { get; }
        object Object { get; }
    }
}
