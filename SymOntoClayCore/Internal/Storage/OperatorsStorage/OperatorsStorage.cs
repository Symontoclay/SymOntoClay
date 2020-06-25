using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
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

        private readonly object _lockObj = new object();

        private readonly Dictionary<KindOfOperator, List<Operator>> _nonIndexedInfo = new Dictionary<KindOfOperator, List<Operator>>();
        private readonly Dictionary<KindOfOperator, List<IndexedOperator>> _indexedInfo = new Dictionary<KindOfOperator, List<IndexedOperator>>();

        /// <inheritdoc/>
        public void Append(Operator op)
        {
#if DEBUG
            Log($"op = {op}");
#endif

            lock(_lockObj)
            {
                var indexedOp = op.GetIndexed(_realStorageContext.EntityDictionary);

#if DEBUG
                Log($"indexedOp = {indexedOp}");
#endif

                var kindOfOperator = indexedOp.KindOfOperator;

                if(_nonIndexedInfo.ContainsKey(kindOfOperator))
                {
                    var list = _nonIndexedInfo[kindOfOperator];

                    if (!list.Contains(op))
                    {
                        list.Add(op);
                        _indexedInfo[kindOfOperator].Add(indexedOp);
                    }
                }
                else
                {
                    _nonIndexedInfo[kindOfOperator] = new List<Operator>() { op };
                    _indexedInfo[kindOfOperator] = new List<IndexedOperator>() { indexedOp };
                }              
            }
        }

        /// <inheritdoc/>
        public IList<IndexedOperator> GetOperatorsDirectly(KindOfOperator kindOfOperator)
        {
#if DEBUG
            Log($"kindOfOperator = {kindOfOperator}");
#endif

            lock (_lockObj)
            {
                if(_indexedInfo.ContainsKey(kindOfOperator))
                {
                    return _indexedInfo[kindOfOperator];
                }

                return new List<IndexedOperator>();
            }
        }
    }
}
