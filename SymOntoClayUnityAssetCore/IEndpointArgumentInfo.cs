using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IEndpointArgumentInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Name { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object DefaultValue { get; }
        int PositionNumber { get; }
        KindOfEndpointParam KindOfParameter { get; }
        bool IsSystemDefiend { get; }
        ParameterInfo ParameterInfo { get; }
    }
}
