///*MIT License

//Copyright (c) 2020 - 2024 Sergiy Tolkachov

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace SymOntoClay.CoreHelper.CollectionsHelpers
//{
//    [Obsolete]
//    public static class CollectionCombinationHelper
//    {
//        public static List<List<T>> Combine<T>(List<List<T>> source)
//        {
//            var resultVector = new List<T>();

//            for (var i = 0; i < source.Count; i++)
//            {
//                resultVector.Add(default);
//            }

//            var n = 0;

//            var result = new List<List<T>>();

//            ProcessCollectionItem(source, n, resultVector, result);

//            return result;
//        }

//        private static void ProcessCollectionItem<T>(List<List<T>> collection, int n, List<T> resultVector, List<List<T>> result)
//        {
//            var isFinal = n + 1 == collection.Count;

//            var targetItems = collection[n];

//            foreach (var targetItem in targetItems)
//            {
//                resultVector[n] = targetItem;

//                if (isFinal)
//                {
//                    result.Add(resultVector.ToList());
//                }
//                else
//                {
//                    ProcessCollectionItem(collection, n + 1, resultVector, result);
//                }
//            }
//        }
//    }
//}
