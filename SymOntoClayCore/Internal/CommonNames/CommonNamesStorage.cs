/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CommonNames
{
    public class CommonNamesStorage : BaseComponent, ICommonNamesStorage
    {
        public CommonNamesStorage(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public StrongIdentifierValue WorldName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedWorldName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue HostName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedHostName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue NpcName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedNpcName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue ClassName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedClassName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue DefaultHolder { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedDefaultHolder { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue SelfSystemVarName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedSelfSystemVarName { get; private set; }

        /// <inheritdoc/>
        public StrongIdentifierValue HostSystemVarName { get; private set; }

        /// <inheritdoc/>
        public IndexedStrongIdentifierValue IndexedHostSystemVarName { get; private set; }

        public void LoadFromSourceCode()
        {
            var dictionary = _context.Dictionary;

            WorldName = NameHelper.CreateName(StandardNamesConstants.WorldTypeName, dictionary);
            IndexedWorldName = WorldName.GetIndexed(_context);

            HostName = NameHelper.CreateName(StandardNamesConstants.HostTypeName, dictionary);
            IndexedHostName = HostName.GetIndexed(_context);

            NpcName = NameHelper.CreateName(StandardNamesConstants.NpcTypeName, dictionary);
            IndexedNpcName = NpcName.GetIndexed(_context);

            ClassName = NameHelper.CreateName(StandardNamesConstants.ClassTypeName, dictionary);
            IndexedClassName = ClassName.GetIndexed(_context);

            DefaultHolder = new StrongIdentifierValue();
            IndexedDefaultHolder = DefaultHolder.GetIndexed(_context);

            SelfSystemVarName = NameHelper.CreateName(StandardNamesConstants.SelfSystemVarName, dictionary);
            IndexedSelfSystemVarName = SelfSystemVarName.GetIndexed(_context);

            HostSystemVarName = NameHelper.CreateName(StandardNamesConstants.HostSystemVarName, dictionary);
            IndexedHostSystemVarName = HostSystemVarName.GetIndexed(_context);

#if IMAGINE_WORKING
            //Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
