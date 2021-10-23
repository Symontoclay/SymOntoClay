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
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.VarStorage
{
    public class VarStorage: BaseComponent, IVarStorage
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

        private Dictionary<StrongIdentifierValue, Value> _systemVariables = new Dictionary<StrongIdentifierValue, Value>();
        private Dictionary<StrongIdentifierValue, Value> _variables = new Dictionary<StrongIdentifierValue, Value>();

        /// <inheritdoc/>
        public void SetSystemValue(StrongIdentifierValue varName, Value value)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
                //Log($"value = {value}");
#endif

                _systemVariables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public Value GetSystemValueDirectly(StrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
#endif

                if(_systemVariables.ContainsKey(varName))
                {
                    return _systemVariables[varName];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public void SetValue(StrongIdentifierValue varName, Value value)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
                //Log($"value = {value}");
#endif

                _variables[varName] = value;
            }
        }

        /// <inheritdoc/>
        public Value GetValueDirectly(StrongIdentifierValue varName)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"varName = {varName}");
#endif

                if (_variables.ContainsKey(varName))
                {
                    return _variables[varName];
                }

                return null;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _systemVariables.Clear();
            _variables.Clear();

            base.OnDisposed();
        }
    }
}
