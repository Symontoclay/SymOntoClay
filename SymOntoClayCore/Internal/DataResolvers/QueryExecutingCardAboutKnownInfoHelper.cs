/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
                var targetRelationVarsInfoDictByKeyOfVar = internalVarsInfoList.ToDictionary(p => p.KeyOfVar, p => p);

                foreach (var initialKnownInfo in externalKnownInfoList)
                {
                    if (inPartFromRelationForProduction)
                    {
                        var position = initialKnownInfo.Position;

                        if (position.HasValue)
                        {
                            var existingVar = targetRelationVarsInfoDictByPosition[position.Value];
                            var resultKnownInfo = initialKnownInfo.Clone();
                            resultKnownInfo.KeyOfVar = existingVar.KeyOfVar;
                            targetKnownInfoList.Add(resultKnownInfo);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        var keyOfVar = initialKnownInfo.KeyOfVar;
                        if (keyOfVar.HasValue)
                        {
                            var keyOfVarValue = keyOfVar.Value;

                            if (targetRelationVarsInfoDictByKeyOfVar.ContainsKey(keyOfVarValue))
                            {
                                var existingVar = targetRelationVarsInfoDictByKeyOfVar[keyOfVarValue];
                                var resultKnownInfo = initialKnownInfo.Clone();
                                resultKnownInfo.KeyOfVar = keyOfVar;
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
