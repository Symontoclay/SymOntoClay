using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public interface ICodeChunksContext
    {
        void Finish();
        void Finish(object result);
    }
}
