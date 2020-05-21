using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IInheritanceStorage
    {
        KindOfStorage Kind { get; }
        void SetInheritance(Name subItem, InheritanceItem inheritanceItem);
        void SetInheritance(Name subItem, InheritanceItem inheritanceItem, bool isPrimary);
    }
}
