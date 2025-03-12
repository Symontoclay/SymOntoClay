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
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Storage.ConstructorsStoraging
{
    public class ConstructorsStorage : BaseSpecificStorage, IConstructorsStorage
    {
        public ConstructorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        { 
        }

        private readonly object _constructorsLockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>> _constructorsDict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<Constructor>>>();

        private readonly object _preConstructorsLockObj = new object();
        private readonly Dictionary<StrongIdentifierValue, PreConstructor> _preConstructorsDict = new Dictionary<StrongIdentifierValue, PreConstructor>();

        /// <inheritdoc/>
        public void Append(IMonitorLogger logger, Constructor constructor)
        {
            lock(_constructorsLockObj)
            {
                constructor.CheckDirty();

                var paramsCountList = StorageHelper.GetParamsCountList(logger, constructor);

                var targetDict = GetDictByHolders(logger, constructor);

                foreach (var count in paramsCountList)
                {
                    if (targetDict.ContainsKey(count))
                    {
                        var targetList = targetDict[count];

                        StorageHelper.RemoveSameItems(logger, targetList, constructor);

                        targetList.Add(constructor);
                    }
                    else
                    {
                        targetDict[count] = new List<Constructor>() { constructor };
                    }
                }
            }
        }

        private Dictionary<int, List<Constructor>> GetDictByHolders(IMonitorLogger logger, Constructor constructor)
        {
            var holder = constructor.Holder;

            if(_constructorsDict.ContainsKey(holder))
            {
                return _constructorsDict[holder];
            }

            var dict = new Dictionary<int, List<Constructor>>();
            _constructorsDict[holder] = dict;
            return dict;
        }

        private static List<WeightedInheritanceResultItem<Constructor>> _emptyConstructorsList = new List<WeightedInheritanceResultItem<Constructor>>();        

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Constructor>> GetConstructorsDirectly(IMonitorLogger logger, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_constructorsLockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyConstructorsList;
                }

                var result = new List<WeightedInheritanceResultItem<Constructor>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperType;

                    if(_constructorsDict.ContainsKey(targetHolder))
                    {
                        var targetDict = _constructorsDict[targetHolder];

                        if (targetDict.ContainsKey(paramsCount))
                        {
                            var targetList = targetDict[paramsCount];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<Constructor>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public void AppendPreConstructor(IMonitorLogger logger, PreConstructor preConstructor)
        {
            lock(_preConstructorsLockObj)
            {
                preConstructor.CheckDirty();

                _preConstructorsDict[preConstructor.Holder] = preConstructor;
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<PreConstructor>> GetPreConstructorsDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            lock (_preConstructorsLockObj)
            {
                if (_realStorageContext.Disabled)
                {
                    return _emptyPreConstructorsList;
                }

                var result = new List<WeightedInheritanceResultItem<PreConstructor>>();

                foreach (var weightedInheritanceItem in weightedInheritanceItems)
                {
                    var targetHolder = weightedInheritanceItem.SuperType;

                    if(_preConstructorsDict.ContainsKey(targetHolder))
                    {
                        var targetVal = _preConstructorsDict[targetHolder];

                        result.Add(new WeightedInheritanceResultItem<PreConstructor>(targetVal, weightedInheritanceItem));
                    }
                }

                return result;
            }
        }

        private static List<WeightedInheritanceResultItem<PreConstructor>> _emptyPreConstructorsList = new List<WeightedInheritanceResultItem<PreConstructor>>();

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _constructorsDict.Clear();
            _preConstructorsDict.Clear();

            base.OnDisposed();
        }
    }
}
