using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.ComponentsInterfaces
{
    public interface ISerializedWorldRoot
    {
        public List<ISerializedWorldComponent> SerializedWorldComponents { get; }
    }
}
