using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging
{
    public class IdleActionItemsStorage : BaseSpecificStorage, IIdleActionItemsStorage
    {
        public IdleActionItemsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(kind, realStorageContext)
        {
        }

        private readonly object _lockObj = new object();

        private Dictionary<StrongIdentifierValue, List<IdleActionItem>> _itemsDict = new Dictionary<StrongIdentifierValue, List<IdleActionItem>>();

        /// <inheritdoc/>
        public void Append(IdleActionItem item)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"idleActionItem = {item.ToHumanizedString()}");
                //Log($"idleActionItem = {item}");
#endif

                AnnotatedItemHelper.CheckAndFillUpHolder(item, _realStorageContext.MainStorageContext.CommonNamesStorage);

                item.CheckDirty();

                var holder = item.Holder;

                if (_itemsDict.ContainsKey(holder))
                {
                    var targetList = _itemsDict[holder];

                    StorageHelper.RemoveSameItems(targetList, item);

                    targetList.Add(item);
                }
                else
                {
                    _itemsDict[holder] = new List<IdleActionItem>() { item };
                }
            }
        }
    }
}
