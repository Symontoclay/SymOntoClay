/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.MetadataStorage
{
    public class MetadataStorage: BaseLoggedComponent, IMetadataStorage
    {
        public MetadataStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private CodeEntity _mainCodeEntity;

        private Dictionary<StrongIdentifierValue, CodeEntity> _codeEntitiesDict = new Dictionary<StrongIdentifierValue, CodeEntity>();

        /// <inheritdoc/>
        public void Append(CodeEntity codeEntity)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"codeEntity = {codeEntity}");
#endif

                var name = codeEntity.Name;

#if DEBUG
                //Log($"name = {name}");
#endif

                if (_codeEntitiesDict.ContainsKey(name))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (codeEntity.Kind == KindOfCodeEntity.App && codeEntity.CodeFile != null && codeEntity.CodeFile.IsMain)
                    {
                        _mainCodeEntity = codeEntity;
                    }

                    _codeEntitiesDict[name] = codeEntity;
                }              
            }
        }

        /// <inheritdoc/>
        public CodeEntity MainCodeEntity 
        {
            get
            {
                return _mainCodeEntity;
            }
        }
    }
}
