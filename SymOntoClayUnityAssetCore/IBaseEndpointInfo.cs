using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IBaseEndpointInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfEndpointInfo KindOfEndpoint { get; }
        string Name { get; }
        bool NeedMainThread { get; }
        IReadOnlyList<int> Devices { get; }
        IReadOnlyList<string> Friends { get; }
        IReadOnlyList<IEndpointArgumentInfo> Arguments { get; }
    }
}
