/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.TriggersStorage
{
    public class TriggersStorage: BaseComponent, ITriggersStorage
    {
        public TriggersStorage(KindOfStorage kind, RealStorageContext realStorageContext)
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

        private readonly Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>> _nonIndexedSystemEventsInfo = new Dictionary<KindOfSystemEventOfInlineTrigger, Dictionary<StrongIdentifierValue, List<InlineTrigger>>>();

        private readonly Dictionary<StrongIdentifierValue, List<InlineTrigger>> _nonIndexedLogicConditionalsInfo = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>();

        /// <inheritdoc/>
        public void Append(InlineTrigger inlineTrigger)
        {
#if DEBUG
            //Log($"inlineTrigger = {inlineTrigger}");
#endif

            AnnotatedItemHelper.CheckAndFillUpHolder(inlineTrigger, _realStorageContext.MainStorageContext.CommonNamesStorage);

            inlineTrigger.CheckDirty();

            var kind = inlineTrigger.Kind;

            switch (kind)
            {
                case KindOfInlineTrigger.SystemEvent:
                    AppendSystemEvent(inlineTrigger);
                    break;

                case KindOfInlineTrigger.LogicConditional:
                    AppendLogicConditional(inlineTrigger);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private void AppendLogicConditional(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                inlineTrigger.CheckDirty();

#if DEBUG
                //Log($"inlineTrigger.Holder = {inlineTrigger.Holder}");
                //Log($"inlineTrigger.Condition = {DebugHelperForRuleInstance.ToString(inlineTrigger.Condition)}");
#endif

                if (_nonIndexedLogicConditionalsInfo.ContainsKey(inlineTrigger.Holder))
                {
                    var targetList = _nonIndexedLogicConditionalsInfo[inlineTrigger.Holder];

#if DEBUG
                    //Log($"_nonIndexedLogicConditionalsInfo[superName].Count = {_nonIndexedLogicConditionalsInfo[inlineTrigger.Holder].Count}");
                    //Log($"targetList = {targetList.WriteListToString()}");
#endif

                    StorageHelper.RemoveSameItems(targetList, inlineTrigger);

                    targetList.Add(inlineTrigger);
                }
                else
                {
                    _nonIndexedLogicConditionalsInfo[inlineTrigger.Holder] = new List<InlineTrigger>() { inlineTrigger };
                }
            }            
        }

        private void AppendSystemEvent(InlineTrigger inlineTrigger)
        {
            lock (_lockObj)
            {
                var kindOfSystemEvent = inlineTrigger.KindOfSystemEvent;

                if (_nonIndexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _nonIndexedSystemEventsInfo[kindOfSystemEvent];

                    if (dict.ContainsKey(inlineTrigger.Holder))
                    {
                        var targetList = dict[inlineTrigger.Holder];

#if DEBUG
                        //Log($"dict[superName].Count = {dict[inlineTrigger.Holder].Count}");
                        //Log($"targetList = {targetList.WriteListToString()}");
#endif

                        StorageHelper.RemoveSameItems(targetList, inlineTrigger);

                        targetList.Add(inlineTrigger);
                    }
                    else
                    {
                        dict[inlineTrigger.Holder] = new List<InlineTrigger>() { inlineTrigger };
                    }
                }
                else
                {
                    _nonIndexedSystemEventsInfo[kindOfSystemEvent] = new Dictionary<StrongIdentifierValue, List<InlineTrigger>>() { { inlineTrigger.Holder, new List<InlineTrigger>() { inlineTrigger } } };
                }
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetSystemEventsTriggersDirectly(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
#if DEBUG
            //Log($"kindOfSystemEvent = {kindOfSystemEvent}");
#endif

            lock (_lockObj)
            {
                if (_nonIndexedSystemEventsInfo.ContainsKey(kindOfSystemEvent))
                {
                    var dict = _nonIndexedSystemEventsInfo[kindOfSystemEvent];

                    var result = new List<WeightedInheritanceResultItem<InlineTrigger>>();

                    foreach (var weightedInheritanceItem in weightedInheritanceItems)
                    {
                        var targetHolder = weightedInheritanceItem.SuperName;

                        if (dict.ContainsKey(targetHolder))
                        {
                            var targetList = dict[targetHolder];

                            foreach (var targetVal in targetList)
                            {
                                result.Add(new WeightedInheritanceResultItem<InlineTrigger>(targetVal, weightedInheritanceItem));
                            }
                        }
                    }

                    return result;
                }

                return new List<WeightedInheritanceResultItem<InlineTrigger>>();
            }
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetLogicConditionalTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            var result = new List<WeightedInheritanceResultItem<InlineTrigger>>();

            foreach (var weightedInheritanceItem in weightedInheritanceItems)
            {
                var targetHolder = weightedInheritanceItem.SuperName;

#if DEBUG
                //Log($"targetHolder = {targetHolder}");
#endif

                if (_nonIndexedLogicConditionalsInfo.ContainsKey(targetHolder))
                {
                    var targetList = _nonIndexedLogicConditionalsInfo[targetHolder];

#if DEBUG
                    //Log($"targetList.Count = {targetList.Count}");
#endif

                    foreach (var targetVal in targetList)
                    {
                        result.Add(new WeightedInheritanceResultItem<InlineTrigger>(targetVal, weightedInheritanceItem));
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _nonIndexedSystemEventsInfo.Clear();
            _nonIndexedLogicConditionalsInfo.Clear();

            base.OnDisposed();
        }
    }
}
