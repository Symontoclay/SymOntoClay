/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
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
        public void Append(IMonitorLogger logger, CodeItem codeItem)
        {
            lock (_lockObj)
            {
                var name = codeItem.Name;

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

                SetAsMainCodeEntity(logger, codeItem);
            }
        }

        private void SetAsMainCodeEntity(IMonitorLogger logger, CodeItem codeEntity)
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
        public CodeItem GetByName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                if (_codeEntitiesDict.ContainsKey(name))
                {
                    var targetList = _codeEntitiesDict[name];

                    if(targetList.Count == 1)
                    {
                        return targetList.Single();
                    }

                    throw new NotImplementedException("6B8560F3-51D1-4086-B9F9-F8822E4CF832");
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
