using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.OperatorsStorage
{
    public class OperatorsStorage : BaseLoggedComponent, IOperatorsStorage
    {
        public OperatorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private readonly Dictionary<KindOfOperator, Dictionary<Name, List<Operator>>> _nonIndexedInfo = new Dictionary<KindOfOperator, Dictionary<Name, List<Operator>>>();
        private readonly Dictionary<KindOfOperator, Dictionary<Name, List<IndexedOperator>>> _indexedInfo = new Dictionary<KindOfOperator, Dictionary<Name, List<IndexedOperator>>>();

        /// <inheritdoc/>
        public void Append(Operator op)
        {
#if DEBUG
            //Log($"op = {op}");
#endif

            AnnotatedItemHelper.CheckAndFillHolder(op, _realStorageContext.CommonNamesStorage);

            lock(_lockObj)
            {
                var indexedOp = op.GetIndexed(_realStorageContext.EntityDictionary);

#if DEBUG
                //Log($"indexedOp = {indexedOp}");
#endif

                var kindOfOperator = indexedOp.KindOfOperator;

                if(_nonIndexedInfo.ContainsKey(kindOfOperator))
                {
                    var dict = _nonIndexedInfo[kindOfOperator];
                    var indexedDict = _indexedInfo[kindOfOperator];

                    if (dict.ContainsKey(op.Holder))
                    {
                        var list = dict[op.Holder];

                        if (!list.Contains(op))
                        {
                            list.Add(op);

                            indexedDict[indexedOp.Holder].Add(indexedOp);
                        }
                    }
                    else
                    {
                        dict[op.Holder] = new List<Operator>() { op };
                        indexedDict[indexedOp.Holder] = new List<IndexedOperator>() { indexedOp };
                    }
                }
                else
                {
                    _nonIndexedInfo[kindOfOperator] = new Dictionary<Name, List<Operator>>() { { op.Holder, new List<Operator>() { op } } };
                    _indexedInfo[kindOfOperator] = new Dictionary<Name, List<IndexedOperator>>() { { indexedOp.Holder, new List<IndexedOperator>() { indexedOp } } };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedOperator>> GetOperatorsDirectly(KindOfOperator kindOfOperator, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfOperator = {kindOfOperator}");
#endif

            lock (_lockObj)
            {
                if(_indexedInfo.ContainsKey(kindOfOperator))
                {
                    var dict = _indexedInfo[kindOfOperator];

                    var result = new List<WeightedInheritanceResultItem<IndexedOperator>>();

                    foreach(var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach(var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<IndexedOperator>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedOperator>>();
            }
        }
    }
}
