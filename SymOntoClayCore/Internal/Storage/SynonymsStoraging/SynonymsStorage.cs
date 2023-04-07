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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.Core.Internal.Storage.SynonymsStoraging
{
    public class SynonymsStorage: BaseSpecificStorage, ISynonymsStorage
    {
        public SynonymsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, List<StrongIdentifierValue>> _synonymsDict = new Dictionary<StrongIdentifierValue, List<StrongIdentifierValue>>();

        /// <inheritdoc/>
        public void Append(Synonym synonym)
        {
            lock (_lockObj)
            {
                synonym.CheckDirty();

                var name = synonym.Name;
                var obj = synonym.Object;

                AddWord(name, obj);
                AddWord(obj, name);
            }
        }

        private void AddWord(StrongIdentifierValue name, StrongIdentifierValue obj)
        {
            if(_synonymsDict.ContainsKey(name))
            {
                var targetList = _synonymsDict[name];

                if(!targetList.Contains(obj))
                {
                    targetList.Add(obj);
                }

                return;
            }

            _synonymsDict[name] = new List<StrongIdentifierValue>() { obj };
        }

        /// <inheritdoc/>
        public IList<StrongIdentifierValue> GetSynonymsDirectly(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                if (_synonymsDict.ContainsKey(name))
                {
                    return _synonymsDict[name];
                }

                return null;
            }
        }
    }
}
