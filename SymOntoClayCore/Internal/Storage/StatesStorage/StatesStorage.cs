using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<StateDef, List<ActivationInfoOfStateDef>> _activationInfoDict = new Dictionary<StateDef, List<ActivationInfoOfStateDef>>();
        private List<ActivationInfoOfStateDef> _activationInfoList = new List<ActivationInfoOfStateDef>();
        private StrongIdentifierValue _defaultStateName;
        private readonly List<MutuallyExclusiveStatesSet> _mutuallyExclusiveStatesSetsList = new List<MutuallyExclusiveStatesSet>();

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

                    var removedItemsList = StorageHelper.RemoveSameItems(targetList, state);

                    targetList.Add(state);

                    if(removedItemsList.Any())
                    {
                        foreach(var removedItem in removedItemsList)
                        {
                            _activationInfoDict.Remove(removedItem);
                        }

                        _activationInfoList = _activationInfoDict.Values.SelectMany(p => p).ToList();
                    }
                }
                else
                {
                    dict[name] = new List<StateDef> { state };
                }

                if(state.ActivatingClauses.Any())
                {
                    var activatingInfoList = new List<ActivationInfoOfStateDef>();

                    foreach (var activatingClause in state.ActivatingClauses)
                    {
                        activatingInfoList.Add(new ActivationInfoOfStateDef(state, activatingClause));
                    }

                    _activationInfoDict[state] = activatingInfoList;

                    _activationInfoList = _activationInfoDict.Values.SelectMany(p => p).ToList();
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

        /// <inheritdoc/>
        public StrongIdentifierValue GetDefaultStateNameDirectly()
        {
            lock (_lockObj)
            {
                return _defaultStateName;
            }
        }

        /// <inheritdoc/>
        public List<ActivationInfoOfStateDef> GetActivationInfoOfStateListDirectly()
        {
            lock (_lockObj)
            {
                return _activationInfoList;
            }
        }

        /// <inheritdoc/>
        public void Append(MutuallyExclusiveStatesSet mutuallyExclusiveStatesSet)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"mutuallyExclusiveStatesSet = {mutuallyExclusiveStatesSet}");
#endif
                if(_mutuallyExclusiveStatesSetsList.Contains(mutuallyExclusiveStatesSet))
                {
                    return;
                }

                _mutuallyExclusiveStatesSetsList.Add(mutuallyExclusiveStatesSet);
            }
        }

        public List<MutuallyExclusiveStatesSet> GetMutuallyExclusiveStatesSetsListDirectly()
        {
            lock (_lockObj)
            {
                return _mutuallyExclusiveStatesSetsList;
            }
        }
    }
}
