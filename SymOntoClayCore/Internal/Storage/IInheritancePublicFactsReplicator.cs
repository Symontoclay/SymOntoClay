using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public interface IInheritancePublicFactsReplicator
    {
        void ProcessChangeInheritance(StrongIdentifierValue subName, StrongIdentifierValue superName);
    }
}
