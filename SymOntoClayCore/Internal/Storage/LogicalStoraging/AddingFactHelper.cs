/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStoraging
{
    public static class AddingFactHelper
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static IAddFactOrRuleResult CallEvent(MulticastDelegate onAddingFactEvent, RuleInstance ruleInstance, FuzzyLogicResolver fuzzyLogicResolver, LocalCodeExecutionContext localCodeExecutionContext, IEntityLogger logger)
        {
            var resultsOfCallList = new List<IAddFactOrRuleResult>();

            var rawObligationsModalitiesList = new List<Value>();
            var rawSelfObligationsModalitiesList = new List<Value>();

            foreach (var item in onAddingFactEvent.GetInvocationList())
            {
                try
                {
                    var rawResultOfCall = item.DynamicInvoke(ruleInstance);

#if DEBUG
                    //_gbcLogger.Info($"rawResultOfCall = {rawResultOfCall}");
#endif

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

                    var mutablePart = resultOfCall.MutablePart;

                    if (mutablePart != null)
                    {
                        rawObligationsModalitiesList.Add(mutablePart.ObligationModality);
                        rawSelfObligationsModalitiesList.Add(mutablePart.SelfObligationModality);
                    }
                }
                catch(Exception e)
                {
                    logger.Error(e.ToString());
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

            if (resultsOfCallList.All(p => p.MutablePart == null))
            {
                return resultsOfCallList.First();
            }

            var result = new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept
            };

            var resultMutablePart = new MutablePartOfRuleInstance() { Parent = ruleInstance };
            result.MutablePart = resultMutablePart;

            resultMutablePart.ObligationModality = MathValueHelper.Max(rawObligationsModalitiesList.Select(p => fuzzyLogicResolver.Resolve(p, localCodeExecutionContext)));
            resultMutablePart.SelfObligationModality = MathValueHelper.Max(rawSelfObligationsModalitiesList.Select(p => fuzzyLogicResolver.Resolve(p, localCodeExecutionContext)));

            return result;
        }
    }
}
