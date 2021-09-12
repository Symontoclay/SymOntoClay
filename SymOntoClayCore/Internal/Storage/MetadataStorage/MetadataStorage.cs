/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

        private Dictionary<StrongIdentifierValue, List<CodeEntity>> _codeEntitiesDict = new Dictionary<StrongIdentifierValue, List<CodeEntity>>();

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

                if(name == null)
                {
                    return;
                }

                if (_codeEntitiesDict.ContainsKey(name))
                {
                    var targetList = _codeEntitiesDict[name];

                    if(!targetList.Contains(codeEntity))
                    {
                        targetList.Add(codeEntity);
                    }
                }
                else
                {
                    _codeEntitiesDict[name] = new List<CodeEntity>() { codeEntity };
                }

                SetAsMainCodeEntity(codeEntity);
            }
        }

        private void SetAsMainCodeEntity(CodeEntity codeEntity)
        {
            if (codeEntity.Kind == KindOfCodeEntity.App && codeEntity.CodeFile != null && codeEntity.CodeFile.IsMain)
            {
                _mainCodeEntity = codeEntity;
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
