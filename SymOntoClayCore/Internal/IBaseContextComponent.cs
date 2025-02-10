using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IBaseContextComponent
    {
        void LinkWithOtherBaseContextComponents();
        void Init();
    }
}
