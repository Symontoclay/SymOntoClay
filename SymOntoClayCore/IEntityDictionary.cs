using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IEntityDictionary
    {
        string Name { get; }
        ulong GetKey(string name);
        string GetName(ulong key);
#if DEBUG
        string GetDbgStr();
#endif
    }
}
