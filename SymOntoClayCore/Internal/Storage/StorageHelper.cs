using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public static class StorageHelper
    {
        public static void RemoveSameItems<T>(List<T> targetList, T item)
            where T: IObjectWithLongHashCodes
        {
            var targetLongHashCode = item.GetLongHashCode();
            var targetLongConditionalHashCode = item.GetLongConditionalHashCode();

#if DEBUG
            //Log($"targetLongHashCode = {targetLongHashCode}");
#endif

            var itemsWithTheSameLongHashCodeList = targetList.Where(p => p.GetLongHashCode() == targetLongHashCode && p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

#if DEBUG
            //Log($"itemsWithTheSameLongHashCodeList = {itemsWithTheSameLongHashCodeList.WriteListToString()}");
#endif

            foreach (var itemWithTheSameLongHashCode in itemsWithTheSameLongHashCodeList)
            {
                targetList.Remove(itemWithTheSameLongHashCode);
            }
        }
    }
}
