/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Storage.VarStorage
{
    public class VarStorage: BaseComponent, IVarStorage
    {
        public VarStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;

            _parentVarStoragesList = realStorageContext.Parents.Select(p => p.VarStorage).ToList();

            foreach (var parentStorage in _parentVarStoragesList)
            {
                parentStorage.OnChangedWithKeys += VarStorage_OnChangedWithKeys;
            }

            realStorageContext.OnAddParentStorage += RealStorageContext_OnAddParentStorage;
            realStorageContext.OnRemoveParentStorage += RealStorageContext_OnRemoveParentStorage;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<StrongIdentifierValue> OnChangedWithKeys;

        private List<IVarStorage> _parentVarStoragesList = new List<IVarStorage>();

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Var>>> _variablesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<Var>>>();
        private readonly Dictionary<StrongIdentifierValue, Var> _localVariablesDict = new Dictionary<StrongIdentifierValue, Var>();

        private Dictionary<StrongIdentifierValue, Value> _systemVariables = new Dictionary<StrongIdentifierValue, Value>();
        
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
        public void Append(Var varItem)
        {
            lock (_lockObj)
            {
                NAppendVar(varItem);
            }
        }

        private void NAppendVar(Var varItem)
        {
#if DEBUG
            //Log($"varItem = {varItem}");
#endif

            if (varItem.TypeOfAccess != TypeOfAccess.Local)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(varItem, _realStorageContext.MainStorageContext.CommonNamesStorage);
            }

            varItem.OnChanged += VarItem_OnChanged;

            varItem.CheckDirty();

            var name = varItem.Name;

            if (varItem.TypeOfAccess == TypeOfAccess.Local)
            {
                _localVariablesDict[name] = varItem;

                return;
            }

            var holder = varItem.Holder;

            Dictionary<StrongIdentifierValue, List<Var>> dict = null;

            if (_variablesDict.ContainsKey(holder))
            {
                dict = _variablesDict[holder];
            }
            else
            {
                dict = new Dictionary<StrongIdentifierValue, List<Var>>();
                _variablesDict[holder] = dict;
            }

            if (dict.ContainsKey(name))
            {
                var targetList = dict[name];

                StorageHelper.RemoveSameItems(targetList, varItem);

                targetList.Add(varItem);
            }
            else
            {
                dict[name] = new List<Var> { varItem };
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Var>> GetVarDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                var result = new List<WeightedInheritanceResultItem<Var>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

                    if(_variablesDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _variablesDict[targetHolder];

#if DEBUG
                        //Log($"targetDict.Count = {targetDict.Count}");
#endif

                        if (targetDict.ContainsKey(name))
                        {
                            var targetList = targetDict[name];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<Var>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                if(_localVariablesDict.ContainsKey(name))
                {
                    var targetVar = _localVariablesDict[name];

                    result.Add(new WeightedInheritanceResultItem<Var>(targetVar, null));
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public Var GetLocalVarDirectly(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                if (_localVariablesDict.ContainsKey(name))
                {
                    return _localVariablesDict[name];
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

                if (_localVariablesDict.ContainsKey(varName))
                {
                    _localVariablesDict[varName].Value = value;
                    return;
                }

                var varItem = new Var();
                varItem.Name = varName;
                
                varItem.TypeOfAccess = TypeOfAccess.Local;

                NAppendVar(varItem);

                varItem.Value = value;
            }
        }

        private void VarItem_OnChanged(StrongIdentifierValue name)
        {
#if DEBUG
            Log($"name = {name}");
#endif
            EmitOnChanged(name);
        }

        protected void EmitOnChanged(StrongIdentifierValue varName)
        {
#if DEBUG
            Log($"varName = {varName}");
#endif

            Task.Run(() => {
                OnChanged?.Invoke();
            });

            Task.Run(() => {
                OnChangedWithKeys?.Invoke(varName);
            });
        }

        private void VarStorage_OnChangedWithKeys(StrongIdentifierValue varName)
        {
#if DEBUG
            Log($"varName = {varName}");
#endif

            EmitOnChanged(varName);
        }

        private void RealStorageContext_OnRemoveParentStorage(IStorage storage)
        {
            var varStroage = storage.VarStorage;
            varStroage.OnChangedWithKeys -= VarStorage_OnChangedWithKeys;

            _parentVarStoragesList.Remove(varStroage);
        }

        private void RealStorageContext_OnAddParentStorage(IStorage storage)
        {
            var varStroage = storage.VarStorage;
            varStroage.OnChangedWithKeys += VarStorage_OnChangedWithKeys;

            _parentVarStoragesList.Add(varStroage);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var parentStorage in _parentVarStoragesList)
            {
                parentStorage.OnChangedWithKeys -= VarStorage_OnChangedWithKeys;
            }

            _systemVariables.Clear();
            _variablesDict.Clear();
            _localVariablesDict.Clear();

            base.OnDisposed();
        }
    }
}
