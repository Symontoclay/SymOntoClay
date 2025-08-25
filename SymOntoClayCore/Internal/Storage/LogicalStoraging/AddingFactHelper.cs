/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public static class AddingFactHelper
    {
        public static IAddFactOrRuleResult CallEvent(IMonitorLogger logger, IList<IOnAddingFactHandler> onAddingFactHandlers, RuleInstance ruleInstance, FuzzyLogicResolver fuzzyLogicResolver, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            var resultsOfCallList = new List<IAddFactOrRuleResult>();

            var changedRuleInstancesList = new List<RuleInstance>();

            foreach (var item in onAddingFactHandlers)
            {
                try
                {
                    var rawResultOfCall = item.OnAddingFact(logger, ruleInstance);

                    if (rawResultOfCall == null)
                    {
                        continue;
                    }

                    var resultOfCall = (IAddFactOrRuleResult)rawResultOfCall;

                    if (resultOfCall.KindOfResult == KindOfAddFactOrRuleResult.Reject)
                    {
                        return resultOfCall;
                    }

                    resultsOfCallList.Add(resultOfCall);

                    var changedRuleInstance = resultOfCall.ChangedRuleInstance;

                    if (changedRuleInstance != null)
                    {
                        changedRuleInstancesList.Add(changedRuleInstance);
                    }
                }
                catch(Exception e)
                {
                    logger.Error("57CBAC4F-F210-40B2-AD69-090EDE744E29", e);
                }
            }

            if (!resultsOfCallList.Any())
            {
                return null;
            }

            if (resultsOfCallList.Count == 1)
            {
                return resultsOfCallList.Single();
            }

            var result = new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept
            };

            result.ChangedRuleInstance = changedRuleInstancesList.Any() ? changedRuleInstancesList.First() : ruleInstance;

            return result;
        }
    }
}
