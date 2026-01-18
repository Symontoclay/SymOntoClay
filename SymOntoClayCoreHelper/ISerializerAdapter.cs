using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public interface ISerializerAdapter
    {
        byte[] Serialize<T>(T obj);
        T Deserialize<T>(byte[] data);
    }
}
