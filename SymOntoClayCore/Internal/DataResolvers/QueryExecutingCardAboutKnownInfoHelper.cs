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

using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public static class QueryExecutingCardAboutKnownInfoHelper
    {
        public static MergingResultOfTwoQueryExecutingCardAboutKnownInfoLists Merge(IList<QueryExecutingCardAboutKnownInfo> internalKnownInfoList, IList<QueryExecutingCardAboutVar> internalVarsInfoList, IList<QueryExecutingCardAboutKnownInfo> externalKnownInfoList, bool inPartFromRelationForProduction)
        {
            var result = new MergingResultOfTwoQueryExecutingCardAboutKnownInfoLists();
            var targetKnownInfoList = new List<QueryExecutingCardAboutKnownInfo>();

            if (externalKnownInfoList.IsNullOrEmpty())
            {
                targetKnownInfoList = internalKnownInfoList.ToList();
            }
            else
            {
                var currentKnownInfoDict = internalKnownInfoList.ToDictionary(p => p.Position, p => p);
                var targetRelationVarsInfoDictByPosition = internalVarsInfoList.ToDictionary(p => p.Position, p => p);
                var targetRelationVarsInfoDictByKeyOfVar = internalVarsInfoList.ToDictionary(p => p.NameOfVar, p => p);

                foreach (var initialKnownInfo in externalKnownInfoList)
                {
                    if (inPartFromRelationForProduction)
                    {
                        var position = initialKnownInfo.Position;

                        if (position.HasValue)
                        {
                            var existingVar = targetRelationVarsInfoDictByPosition[position.Value];
                            var resultKnownInfo = initialKnownInfo.Clone();
                            resultKnownInfo.NameOfVar = existingVar.NameOfVar;
                            targetKnownInfoList.Add(resultKnownInfo);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        var keyOfVar = initialKnownInfo.NameOfVar;
                        if (keyOfVar != null && !keyOfVar.IsEmpty)
                        {
                            var keyOfVarValue = keyOfVar;

                            if (targetRelationVarsInfoDictByKeyOfVar.ContainsKey(keyOfVarValue))
                            {
                                var existingVar = targetRelationVarsInfoDictByKeyOfVar[keyOfVarValue];
                                var resultKnownInfo = initialKnownInfo.Clone();
                                resultKnownInfo.NameOfVar = keyOfVar;
                                resultKnownInfo.Position = existingVar.Position;
                                targetKnownInfoList.Add(resultKnownInfo);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }

            result.KnownInfoList = targetKnownInfoList;
            result.IsSuccess = true;
            return result;
        }
    }
}
