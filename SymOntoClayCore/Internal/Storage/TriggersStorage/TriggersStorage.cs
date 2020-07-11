using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.TriggersStorage
{
    public class TriggersStorage: BaseLoggedComponent, ITriggersStorage
    {
        public TriggersStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<Name, List<InlineTrigger>>> _nonIndexedSystemEventsInfo = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<Name, List<InlineTrigger>>>();
        private readonly Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<Name, List<IndexedInlineTrigger>>> _indexedSystemEventsInfo = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<Name, List<IndexedInlineTrigger>>>();

        /// <inheritdoc/>
        public void Append(InlineTrigger inlineTrigger)
        {
#if DEBUG
            Log($"inlineTrigger = {inlineTrigger}");
#endif

            AnnotatedItemHelper.CheckAndFillHolder(inlineTrigger, _realStorageContext.CommonNamesStorage);

            var kind = inlineTrigger.Kind;

            switch (kind)
            {
                case KindOfInlineTrigger.SystemEvent:
                    AppendSystemEvent(inlineTrigger);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void AppendSystemEvent(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                var indexedItem = inlineTrigger.GetIndexed(_realStorageContext.EntityDictionary, _realStorageContext.Compiler);

#if DEBUG
                Log($"indexedItem = {indexedItem}");
#endif

                var kindOfSystemEvent = inlineTrigger.KindOfSystemEvent;

                if (_nonIndexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _nonIndexedSystemEventsInfo[kindOfSystemEvent];
                    var indexedDict = _indexedSystemEventsInfo[kindOfSystemEvent];

                    if (dict.ContainsKey(inlineTrigger.Holder))
                    {
                        var list = dict[inlineTrigger.Holder];

                        if (!list.Contains(inlineTrigger))
                        {
                            list.Add(inlineTrigger);

                            indexedDict[indexedItem.Holder].Add(indexedItem);
                        }
                    }
                    else
                    {
                        dict[inlineTrigger.Holder] = new List<InlineTrigger>() { inlineTrigger };
                        indexedDict[indexedItem.Holder] = new List<IndexedInlineTrigger>() { indexedItem };
                    }
                }
                else
                {
                    _nonIndexedSystemEventsInfo[kindOfSystemEvent] = new Dictionary<Name, List<InlineTrigger>>() { { inlineTrigger.Holder, new List<InlineTrigger>() { inlineTrigger } } };
                    _indexedSystemEventsInfo[kindOfSystemEvent] = new Dictionary<Name, List<IndexedInlineTrigger>>() { { indexedItem.Holder, new List<IndexedInlineTrigger>() { indexedItem } } };
                }
            }
            
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<IndexedInlineTrigger>> GetSystemEventsTriggersDirectly(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            Log($"kindOfSystemEvent = {kindOfSystemEvent}");
#endif

            lock (_lockObj)
            {
                if (_indexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _indexedSystemEventsInfo[kindOfSystemEvent];

                    var result = new List<WeightedInheritanceResultItem<IndexedInlineTrigger>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<IndexedInlineTrigger>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<IndexedInlineTrigger>>();
            }
        }
    }
}
