/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ActionsStoraging
{
    public class ActionsStorage: BaseSpecificStorage, IActionsStorage
    {
        public ActionsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>> _actionsDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, ActionDef action)
        {
            lock (_lockObj)
            {
                AnnotatedItemHelper.CheckAndFillUpHolder(logger, action, _realStorageContext.MainStorageContext.CommonNamesStorage);

                action.CheckDirty();

                var holder = action.Holder;

                var namesList = action.NamesList;

                var paramsCountDict = GetParamsCountDict(logger, action);

                var paramsCountList = paramsCountDict.Keys.ToList();

                foreach(var name in namesList)
                {
                    var targetDict = GetDictByNames(logger, holder, name);

                    foreach (var count in paramsCountList)
                    {
                        var operatorsList = paramsCountDict[count];

                        List<ActionPtr> targetList = null;

                        if (targetDict.ContainsKey(count))
                        {
                            targetList = targetDict[count];
                        }
                        else
                        {
                            targetList = new List<ActionPtr>();
                            targetDict[count] = targetList;
                        }

                        foreach(var op in operatorsList)
                        {
                            if (!targetList.Any(p => p.Action == action && p.Operator == op))
                            {
                                targetList.Add(new ActionPtr(action, op));
                            }
                        }
                    }
                }
            }
        }

        private static List<WeightedInheritanceResultItem<ActionPtr>> _emptyActionsList = new List<WeightedInheritanceResultItem<ActionPtr>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<ActionPtr>> GetActionsDirectly(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyActionsList;
                }

                var result = new List<WeightedInheritanceResultItem<ActionPtr>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

                    if (_actionsDict.ContainsKey(targetHolder))
                    {
                        var dict = _actionsDict[targetHolder];

                        if (dict.ContainsKey(name))
                        {
                            var targetDict = dict[name];

                            if (targetDict.ContainsKey(paramsCount))
                            {
                                var targetList = targetDict[paramsCount];

                                foreach (var targetVal in targetList)
                                {
                                    result.Add(new WeightedInheritanceResultItem<ActionPtr>(targetVal, weightedInheritanceItem));
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        private Dictionary<int, List<Operator>> GetParamsCountDict(IMonitorLogger logger, ActionDef action)
        {
            var result = new Dictionary<int, List<Operator>>();

            foreach(var op in action.Operators)
            {
                var paramsCountList = GetParamsCountList(logger, op);

                foreach(var count in paramsCountList)
                {
                    AddToDict(logger, count, result, op);
                }
            }

            return result;
        }

        private void AddToDict(IMonitorLogger logger, int paramCount, Dictionary<int, List<Operator>> dict, Operator @operator)
        {
            if(dict.ContainsKey(paramCount))
            {
                var list = dict[paramCount];

                if(!list.Contains(@operator))
                {
                    list.Add(@operator);
                }

                return;
            }

            dict[paramCount] = new List<Operator>() { @operator };
        }

        private List<int> GetParamsCountList(IMonitorLogger logger, Operator @operator)
        {
            var result = new List<int>();

            var argumentsList = @operator.Arguments;

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count();
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.HasDefaultValue);

            if (totalCount == argumentsWithoutDefaultValueCount)
            {
                result.Add(totalCount);
                return result;
            }

            for (var i = argumentsWithoutDefaultValueCount; i <= totalCount; i++)
            {
                result.Add(i);
            }

            return result;
        }

        private Dictionary<int, List<ActionPtr>> GetDictByNames(IMonitorLogger logger, StrongIdentifierValue holder, StrongIdentifierValue name)
        {
            if (_actionsDict.ContainsKey(holder))
            {
                var dict = _actionsDict[holder];

                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }

                {
                    var targetDict = new Dictionary<int, List<ActionPtr>>();
                    dict[name] = targetDict;
                    return targetDict;
                }
            }

            {
                var dict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>();
                _actionsDict[holder] = dict;
                var targetDict = new Dictionary<int, List<ActionPtr>>();
                dict[name] = targetDict;
                return targetDict;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _actionsDict.Clear();

            base.OnDisposed();
        }
    }
}
