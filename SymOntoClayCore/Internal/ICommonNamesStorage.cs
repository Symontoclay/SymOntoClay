using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ICommonNamesStorage
    {
        StrongIdentifierValue WorldName { get; }
        IndexedStrongIdentifierValue IndexedWorldName { get; }

        StrongIdentifierValue HostName { get; }
        IndexedStrongIdentifierValue IndexedHostName { get; }

        StrongIdentifierValue ApplicationName { get; }
        IndexedStrongIdentifierValue IndexedApplicationName { get; }

        StrongIdentifierValue ClassName { get; }
        IndexedStrongIdentifierValue IndexedClassName { get; }

        StrongIdentifierValue DefaultHolder { get; }
        IndexedStrongIdentifierValue IndexedDefaultHolder { get; }

        StrongIdentifierValue SelfSystemVarName { get; }
        IndexedStrongIdentifierValue IndexedSelfSystemVarName { get; }

        StrongIdentifierValue HostSystemVarName { get; }
        IndexedStrongIdentifierValue IndexedHostSystemVarName { get; }
    }
}
