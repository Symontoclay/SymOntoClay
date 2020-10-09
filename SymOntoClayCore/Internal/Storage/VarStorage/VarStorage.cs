/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.VarStorage
{
    public class VarStorage: BaseLoggedComponent, IVarStorage
    {
        public VarStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private Dictionary<IndexedStrongIdentifierValue, IndexedValue> _indexedSystemVariables = new Dictionary<IndexedStrongIdentifierValue, IndexedValue>();
        private Dictionary<IndexedStrongIdentifierValue, IndexedValue> _indexedVariables = new Dictionary<IndexedStrongIdentifierValue, IndexedValue>();

        /// <inheritdoc/>
        public void SetSystemValue(IndexedStrongIdentifierValue varName, IndexedValue value)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
                //Log($"value = {value}");
#endif

                _indexedSystemVariables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public IndexedValue GetSystemValueDirectly(IndexedStrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
#endif

                if(_indexedSystemVariables.ContainsKey(varName))
                {
                    return _indexedSystemVariables[varName];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void SetValue(IndexedStrongIdentifierValue varName, IndexedValue value)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
                //Log($"value = {value}");
#endif

                _indexedVariables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public IndexedValue GetValueDirectly(IndexedStrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
#endif

                if (_indexedVariables.ContainsKey(varName))
                {
                    return _indexedVariables[varName];
                }

                return null;
            }
        }
    }
}
