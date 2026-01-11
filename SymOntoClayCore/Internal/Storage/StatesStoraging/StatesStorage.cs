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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.StatesStoraging
{
    public class StatesStorage : BaseSpecificStorage, IStatesStorage
    {
        public StatesStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<StateDef>>> _statesDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, List<StateDef>>>();
        private readonly Dictionary<StateDef, List<ActivationInfoOfStateDef>> _activationInfoDict = new Dictionary<StateDef, List<ActivationInfoOfStateDef>>();
        private List<StateDef> _statesList = new List<StateDef>();
        private List<ActivationInfoOfStateDef> _activationInfoList = new List<ActivationInfoOfStateDef>();
        private List<StrongIdentifierValue> _stateNamesList = new List<StrongIdentifierValue>();
        private StrongIdentifierValue _defaultStateName;
        private readonly List<MutuallyExclusiveStatesSet> _mutuallyExclusiveStatesSetsList = new List<MutuallyExclusiveStatesSet>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, StateDef state)
        {
            lock (_lockObj)
            {
                if (_statesList.Contains(state))
                {
                    return;
                }

                AnnotatedItemHelper.CheckAndFillUpHolder(logger, state, _realStorageContext.MainStorageContext.CommonNamesStorage);

                state.CheckDirty();

                var name = state.Name;

                var holder = state.Holder;

                _statesList.Add(state);

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

                    var removedItemsList = StorageHelper.RemoveSameItems(logger, targetList, state);

                    targetList.Add(state);

                    if(removedItemsList.Any())
                    {
                        foreach(var removedItem in removedItemsList)
                        {
                            _activationInfoDict.Remove(removedItem);
                            _statesList.Remove(removedItem);
                        }

                        _activationInfoList = _activationInfoDict.Values.SelectMany(p => p).ToList();                        
                    }
                }
                else
                {
                    dict[name] = new List<StateDef> { state };

                    _stateNamesList.Add(name);
                }

                if(state.ActivatingConditions.Any())
                {
                    var activatingInfoList = new List<ActivationInfoOfStateDef>();

                    foreach (var activatingCondition in state.ActivatingConditions)
                    {
                        activatingInfoList.Add(new ActivationInfoOfStateDef(state, activatingCondition));
                    }

                    _activationInfoDict[state] = activatingInfoList;

                    _activationInfoList = _activationInfoDict.Values.SelectMany(p => p).ToList();
                }
            }
        }

        private static List<WeightedInheritanceResultItem<StateDef>> _emptyWeightedStatesList = new List<WeightedInheritanceResultItem<StateDef>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<StateDef>> GetStatesDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyWeightedStatesList;
                }

                var result = new List<WeightedInheritanceResultItem<StateDef>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

                    if (_statesDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _statesDict[targetHolder];

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

        private static List<StrongIdentifierValue> _emptyAllStateNamesList = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        public List<StrongIdentifierValue> AllStateNames(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyAllStateNamesList;
                }

                return _stateNamesList;
            }
        }

        private static List<StateDef> _emptyAllStatesList = new List<StateDef>();

        /// <inheritdoc/>
        public List<StateDef> GetAllStatesListDirectly(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyAllStatesList;
                }

                return _statesList;
            }
        }

        /// <inheritdoc/>
        public void SetDefaultStateName(IMonitorLogger logger, StrongIdentifierValue name)
        {
            lock (_lockObj)
            {
                _defaultStateName = name;
            }
        }

        /// <inheritdoc/>
        public StrongIdentifierValue GetDefaultStateNameDirectly(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return null;
                }

                return _defaultStateName;
            }
        }

        private static List<ActivationInfoOfStateDef> _emptyActivationInfoOfStateList = new List<ActivationInfoOfStateDef>();

        /// <inheritdoc/>
        public List<ActivationInfoOfStateDef> GetActivationInfoOfStateListDirectly(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyActivationInfoOfStateList;
                }

                return _activationInfoList;
            }
        }

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, MutuallyExclusiveStatesSet mutuallyExclusiveStatesSet)
        {
            lock (_lockObj)
            {
                if(_mutuallyExclusiveStatesSetsList.Contains(mutuallyExclusiveStatesSet))
                {
                    return;
                }

                _mutuallyExclusiveStatesSetsList.Add(mutuallyExclusiveStatesSet);
            }
        }

        private static List<MutuallyExclusiveStatesSet> _emptyMutuallyExclusiveStatesSetsList = new List<MutuallyExclusiveStatesSet>();

        /// <inheritdoc/>
        public List<MutuallyExclusiveStatesSet> GetMutuallyExclusiveStatesSetsListDirectly(IMonitorLogger logger)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyMutuallyExclusiveStatesSetsList;
                }

                return _mutuallyExclusiveStatesSetsList;
            }
        }
    }
}
