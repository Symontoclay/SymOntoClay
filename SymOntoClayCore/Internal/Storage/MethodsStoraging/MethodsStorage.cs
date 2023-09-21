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
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Linq;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Storage.MethodsStoraging
{
    public class MethodsStorage: BaseSpecificStorage, IMethodsStorage
    {
        public MethodsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>> _namedFunctionsDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>>();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>> _localNamedFunctionsDict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, NamedFunction namedFunction)
        {
            lock (_lockObj)
            {
                if (namedFunction.TypeOfAccess != TypeOfAccess.Local)
                {
                    AnnotatedItemHelper.CheckAndFillUpHolder(logger, namedFunction, _realStorageContext.MainStorageContext.CommonNamesStorage);
                }

                namedFunction.CheckDirty();

                var namedFunctionName = namedFunction.Name;

                var paramsCountList = StorageHelper.GetParamsCountList(logger, namedFunction);

                var targetDict = GetDictByNames(logger, namedFunction);

                foreach (var count in paramsCountList)
                {
                    if (targetDict.ContainsKey(count))
                    {
                        var targetList = targetDict[count];

                        StorageHelper.RemoveSameItems(logger, targetList, namedFunction);

                        targetList.Add(namedFunction);
                    }
                    else
                    {
                        targetDict[count] = new List<NamedFunction>() { namedFunction };
                    }
                }
            }
        }

        private Dictionary<int, List<NamedFunction>> GetDictByNames(IMonitorLogger logger, NamedFunction namedFunction)
        {
            var name = namedFunction.Name;

            if(namedFunction.TypeOfAccess == TypeOfAccess.Local)
            {
                if(_localNamedFunctionsDict.ContainsKey(name))
                {
                    return _localNamedFunctionsDict[name];
                }

                var targetDict = new Dictionary<int, List<NamedFunction>>();
                _localNamedFunctionsDict[name] = targetDict;
                return targetDict;
            }

            var holder = namedFunction.Holder;

            if (_namedFunctionsDict.ContainsKey(holder))
            {
                var dict = _namedFunctionsDict[holder];

                if(dict.ContainsKey(name))
                {
                    return dict[name];
                }

                {
                    var targetDict = new Dictionary<int, List<NamedFunction>>();
                    dict[name] = targetDict;
                    return targetDict;
                }
            }

            {
                var dict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>();
                _namedFunctionsDict[holder] = dict;
                var targetDict = new Dictionary<int, List<NamedFunction>>();
                dict[name] = targetDict;
                return targetDict;
            }
        }

        private static List<WeightedInheritanceResultItem<NamedFunction>> _emptyNamedFunctionsList = new List<WeightedInheritanceResultItem<NamedFunction>>();

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<NamedFunction>> GetNamedFunctionsDirectly(IMonitorLogger logger, StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyNamedFunctionsList;
                }

                var result = new List<WeightedInheritanceResultItem<NamedFunction>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

                    if(_namedFunctionsDict.ContainsKey(targetHolder))
                    {
                        var dict = _namedFunctionsDict[targetHolder];

                        if(dict.ContainsKey(name))
                        {
                            var targetDict = dict[name];

                            if(targetDict.ContainsKey(paramsCount))
                            {
                                var targetList = targetDict[paramsCount];

                                foreach (var targetVal in targetList)
                                {
                                    result.Add(new WeightedInheritanceResultItem<NamedFunction>(targetVal, weightedInheritanceItem));
                                }
                            }
                        }
                    }
                }

                if(_localNamedFunctionsDict.ContainsKey(name))
                {
                    var targetDict = _localNamedFunctionsDict[name];

                    if (targetDict.ContainsKey(paramsCount))
                    {
                        var targetList = targetDict[paramsCount];

                        foreach (var targetVal in targetList)
                        {
                            result.Add(new WeightedInheritanceResultItem<NamedFunction>(targetVal, null));
                        }
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _namedFunctionsDict.Clear();
            _localNamedFunctionsDict.Clear();

            base.OnDisposed();
        }
    }
}
