using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILogicalStorage
    {
        KindOfStorage Kind { get; }
    }
}
