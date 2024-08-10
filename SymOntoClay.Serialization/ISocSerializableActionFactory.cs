using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization
{
    public interface ISocSerializableActionFactory
    {
        string Id { get; }
        object GetAction();
    }
}
