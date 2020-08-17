using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public interface IPlatformTypesConvertorsRegistry
    {
        void AddConvertor(IPlatformTypesConvertor convertor);
        bool CanConvert(Type source, Type dest);
        object Convert(Type sourceType, Type destType, object sourceValue);
    }
}
