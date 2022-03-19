using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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
    }
}
