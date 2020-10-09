/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
