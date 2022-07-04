using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.CoreHelper.CollectionsHelpers
{
    public static class CollectionCombinationHelper
    {
        public static List<List<T>> Combine<T>(List<List<T>> source)
        {
            var resultVector = new List<T>();

            for (var i = 0; i < source.Count; i++)
            {
                resultVector.Add(default);
            }

            var n = 0;

            var result = new List<List<T>>();

            ProcessCollectionItem(source, n, resultVector, result);

            return result;
        }

        private static void ProcessCollectionItem<T>(List<List<T>> collection, int n, List<T> resultVector, List<List<T>> result)
        {
            var isFinal = n + 1 == collection.Count;

            var targetItems = collection[n];

            foreach (var targetItem in targetItems)
            {
                resultVector[n] = targetItem;

                if (isFinal)
                {
                    result.Add(resultVector.ToList());
                }
                else
                {
                    ProcessCollectionItem(collection, n + 1, resultVector, result);
                }
            }
        }
    }
}
