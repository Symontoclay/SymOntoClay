/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.MetadataStoraging
{
    public class MetadataStorage: BaseSpecificStorage, IMetadataStorage
    {
        public MetadataStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private CodeItem _mainCodeEntity;

        private Dictionary<StrongIdentifierValue, List<CodeItem>> _codeEntitiesDict = new Dictionary<StrongIdentifierValue, List<CodeItem>>();

        /// <inheritdoc/>
        public void Append(CodeItem codeItem)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"codeEntity = {codeEntity}");
#endif

                var name = codeItem.Name;

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

                    if(!targetList.Contains(codeItem))
                    {
                        targetList.Add(codeItem);
                    }
                }
                else
                {
                    _codeEntitiesDict[name] = new List<CodeItem>() { codeItem };
                }

                SetAsMainCodeEntity(codeItem);
            }
        }

        private void SetAsMainCodeEntity(CodeItem codeEntity)
        {
            if (codeEntity.Kind == KindOfCodeEntity.App && codeEntity.CodeFile != null && codeEntity.CodeFile.IsMain)
            {
                _mainCodeEntity = codeEntity;
            }
        }

        /// <inheritdoc/>
        public CodeItem MainCodeEntity 
        {
            get
            {
                return _mainCodeEntity;
            }
        }

        /// <inheritdoc/>
        public CodeItem GetByName(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                if (_codeEntitiesDict.ContainsKey(name))
                {
                    var targetList = _codeEntitiesDict[name];

                    if(targetList.Count == 1)
                    {
                        return targetList.Single();
                    }

                    throw new NotImplementedException();
                }

                return null;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _codeEntitiesDict.Clear();
            _mainCodeEntity = null;

            base.OnDisposed();
        }
    }
}
