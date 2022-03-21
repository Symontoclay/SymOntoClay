using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.StatesStorage
{
    public class StatesStorage : BaseComponent, IStatesStorage
    {
        public StatesStorage(KindOfStorage kind, RealStorageContext realStorageContext)
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

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<StateDef>>> _statesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<StateDef>>>();
        private StrongIdentifierValue _defaultStateName;

        /// <inheritdoc/>
        public void Append(StateDef state)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"state = {state}");
#endif

                AnnotatedItemHelper.CheckAndFillUpHolder(state, _realStorageContext.MainStorageContext.CommonNamesStorage);

                state.CheckDirty();

                var name = state.Name;

                var holder = state.Holder;

                Dictionary<StrongIdentifierValue, List<StateDef>> dict = null;

                if (_statesDict.ContainsKey(holder))
                {
                    dict = _statesDict[holder];
                }
                else
                {
                    dict = new Dictionary<StrongIdentifierValue, List<StateDef>>();
                    _statesDict[holder] = dict;
                }

                if (dict.ContainsKey(name))
                {
                    var targetList = dict[name];

                    StorageHelper.RemoveSameItems(targetList, state);

                    targetList.Add(state);
                }
                else
                {
                    dict[name] = new List<StateDef> { state };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<StateDef>> GetStatesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                var result = new List<WeightedInheritanceResultItem<StateDef>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

                    if (_statesDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _statesDict[targetHolder];

#if DEBUG
                        //Log($"targetDict.Count = {targetDict.Count}");
#endif

                        if (targetDict.ContainsKey(name))
                        {
                            var targetList = targetDict[name];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<StateDef>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public void SetDefaultStateName(StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
#endif

                _defaultStateName = name;
            }
        }
    }
}
