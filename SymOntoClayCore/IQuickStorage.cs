using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IQuickStorage
    {
        IStorage Storage { get; }
        string InsertFact(string text);
        void RemoveFact(string id);
    }
}
