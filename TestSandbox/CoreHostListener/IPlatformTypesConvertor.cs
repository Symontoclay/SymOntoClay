using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public interface IPlatformTypesConvertor : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        Type PlatformType { get; }
        Type CoreType { get; }
        bool CanConvertToPlatformType { get; }
        bool CanConvertToCoreType { get; }
        object ConvertToPlatformType(object coreObject, IEntityLogger logger);
        object ConvertToCoreType(object platformObject, IEntityLogger logger);
    }
}
