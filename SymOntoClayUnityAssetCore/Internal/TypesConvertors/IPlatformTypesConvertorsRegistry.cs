using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConvertors
{
    public interface IPlatformTypesConvertorsRegistry
    {
        void AddConvertor(IPlatformTypesConvertor convertor);
        bool CanConvert(Type source, Type dest);
        object Convert(Type sourceType, Type destType, object sourceValue);
    }
}
