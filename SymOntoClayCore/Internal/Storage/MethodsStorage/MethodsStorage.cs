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
    public class MethodsStorage: BaseComponent, IMethodsStorage
    {
        public MethodsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>> _namedFunctionsDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>>();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        /// <inheritdoc/>
        public void Append(NamedFunction namedFunction)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"namedFunction = {namedFunction}");
#endif

                AnnotatedItemHelper.CheckAndFillUpHolder(namedFunction, _realStorageContext.MainStorageContext.CommonNamesStorage);

                namedFunction.CheckDirty();

                var namedFunctionName = namedFunction.Name;

                var paramsCountList = GetParamsCountList(namedFunction);

#if DEBUG
                //Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                var targetDict = GetDictByNames(namedFunction);

                foreach (var count in paramsCountList)
                {
                    List<NamedFunction> targetList = null;

                    if (targetDict.ContainsKey(count))
                    {
                        targetList = targetDict[count];
                    }
                    else
                    {
                        targetList = new List<NamedFunction>();
                        targetDict[count] = targetList;
                    }

                    if (!targetList.Contains(namedFunction))
                    {
                        targetList.Add(namedFunction);
                    }
                }
            }
        }

        private Dictionary<int, List<NamedFunction>> GetDictByNames(NamedFunction namedFunction)
        {
            var holder = namedFunction.Holder;

#if DEBUG
            //Log($"holder = {holder}");
#endif

            if(_namedFunctionsDict.ContainsKey(holder))
            {
                var dict = _namedFunctionsDict[holder];

                if(dict.ContainsKey(namedFunction.Name))
                {
                    return dict[namedFunction.Name];
                }

                {
                    var targetDict = new Dictionary<int, List<NamedFunction>>();
                    dict[namedFunction.Name] = targetDict;
                    return targetDict;
                }
            }

            {
                var dict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<NamedFunction>>>();
                _namedFunctionsDict[holder] = dict;
                var targetDict = new Dictionary<int, List<NamedFunction>>();
                dict[namedFunction.Name] = targetDict;
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

            base.OnDisposed();
        }
    }
}
