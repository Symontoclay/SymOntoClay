/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public static class StorageHelper
    {
        public static List<T> RemoveSameItems<T>(List<T> targetList, T item)
            where T: IObjectWithLongHashCodes
        {
            var targetLongHashCode = item.GetLongHashCode();
            var targetLongConditionalHashCode = item.GetLongConditionalHashCode();

            var itemsWithTheSameLongHashCodeList = targetList.Where(p => p.GetLongHashCode() == targetLongHashCode && p.GetLongConditionalHashCode() == targetLongConditionalHashCode).ToList();

            foreach (var itemWithTheSameLongHashCode in itemsWithTheSameLongHashCodeList)
            {
                targetList.Remove(itemWithTheSameLongHashCode);
            }

            return itemsWithTheSameLongHashCodeList;
        }

        public static List<int> GetParamsCountList(Function function)
        {
            var result = new List<int>();

            var argumentsList = function.Arguments;

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count();
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.HasDefaultValue);

            if (totalCount == argumentsWithoutDefaultValueCount)
            {
                result.Add(totalCount);
                return result;
            }

            for (var i = argumentsWithoutDefaultValueCount; i <= totalCount; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}
