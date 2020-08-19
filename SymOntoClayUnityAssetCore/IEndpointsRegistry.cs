using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IEndpointsRegistry
    {
        IList<IEndpointInfo> GetEndpointsInfoListDirectly(string endPointName, int paramsCount);
    }
}
