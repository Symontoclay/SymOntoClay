using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ILogicalSearchStorageContext
    {
        IList<T> Filter<T>(IList<T> source, bool enableModalitiesControll) where T : ILogicalSearchItem;
    }
}
