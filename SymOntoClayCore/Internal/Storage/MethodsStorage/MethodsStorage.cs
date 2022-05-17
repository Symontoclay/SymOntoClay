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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Linq;
using SymOntoClay.Core.Internal.CodeModel.Helpers;

namespace SymOntoClay.Core.Internal.Storage.MethodsStorage
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
        public void Append(NamedFunction namedFunction)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"namedFunction = {namedFunction}");
#endif

                if(namedFunction.TypeOfAccess != TypeOfAccess.Local)
                {
                    AnnotatedItemHelper.CheckAndFillUpHolder(namedFunction, _realStorageContext.MainStorageContext.CommonNamesStorage);
                }

                namedFunction.CheckDirty();

                var namedFunctionName = namedFunction.Name;

                var paramsCountList = GetParamsCountList(namedFunction);

#if DEBUG
                //Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                var targetDict = GetDictByNames(namedFunction);

                foreach (var count in paramsCountList)
                {
                    if (targetDict.ContainsKey(count))
                    {
                        var targetList = targetDict[count];

                        StorageHelper.RemoveSameItems(targetList, namedFunction);

                        targetList.Add(namedFunction);
                    }
                    else
                    {
                        targetDict[count] = new List<NamedFunction>() { namedFunction };
                    }
                }
            }
        }

        private Dictionary<int, List<NamedFunction>> GetDictByNames(NamedFunction namedFunction)
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

#if DEBUG
            //Log($"holder = {holder}");
#endif

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

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<NamedFunction>> GetNamedFunctionsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"name = {name}");
                //Log($"paramsCount = {paramsCount}");
#endif

                var result = new List<WeightedInheritanceResultItem<NamedFunction>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                    //Log($"targetHolder = {targetHolder}");
#endif

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

        private List<int> GetParamsCountList(NamedFunction namedFunction)
        {
            var result = new List<int>();

            var argumentsList = namedFunction.Arguments;

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

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _namedFunctionsDict.Clear();
            _localNamedFunctionsDict.Clear();

            base.OnDisposed();
        }
    }
}
